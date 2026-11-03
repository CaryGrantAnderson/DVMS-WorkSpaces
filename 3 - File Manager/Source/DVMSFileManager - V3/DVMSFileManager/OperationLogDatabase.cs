using System;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace DVMSFileManager
{
    internal sealed class OperationLogDatabase
    {
        public string DatabasePath { get; private set; }

        public OperationLogDatabase()
        {
            string directory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "DB");
            Directory.CreateDirectory(directory);
            DatabasePath = Path.Combine(
                directory, "DVMSFileManagerLog.db");
        }

        public void Initialize()
        {
            using (var connection = Open())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
CREATE TABLE IF NOT EXISTS OperationLog (
    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
    OccurredUtc TEXT NOT NULL,
    Operation TEXT NOT NULL,
    SourcePath TEXT NULL,
    TargetPath TEXT NULL,
    Details TEXT NULL,
    WasSuccessful INTEGER NOT NULL,
    ErrorMessage TEXT NULL,
    WindowsUser TEXT NULL,
    ComputerName TEXT NULL
);
CREATE INDEX IF NOT EXISTS IX_OperationLog_OccurredUtc
    ON OperationLog(OccurredUtc DESC);
CREATE INDEX IF NOT EXISTS IX_OperationLog_Operation
    ON OperationLog(Operation, OccurredUtc DESC);";
                command.ExecuteNonQuery();
            }
        }

        public void Log(
            string operation,
            string sourcePath = null,
            string targetPath = null,
            string details = null,
            bool successful = true,
            string errorMessage = null)
        {
            try
            {
                using (var connection = Open())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
INSERT INTO OperationLog
    (OccurredUtc, Operation, SourcePath, TargetPath, Details,
     WasSuccessful, ErrorMessage, WindowsUser, ComputerName)
VALUES
    (@occurred, @operation, @source, @target, @details,
     @successful, @error, @user, @computer);";
                    command.Parameters.AddWithValue(
                        "@occurred", DateTime.UtcNow.ToString("o"));
                    command.Parameters.AddWithValue(
                        "@operation", operation ?? string.Empty);
                    command.Parameters.AddWithValue(
                        "@source", DbValue(sourcePath));
                    command.Parameters.AddWithValue(
                        "@target", DbValue(targetPath));
                    command.Parameters.AddWithValue(
                        "@details", DbValue(details));
                    command.Parameters.AddWithValue(
                        "@successful", successful ? 1 : 0);
                    command.Parameters.AddWithValue(
                        "@error", DbValue(errorMessage));
                    command.Parameters.AddWithValue(
                        "@user", DbValue(Environment.UserName));
                    command.Parameters.AddWithValue(
                        "@computer", DbValue(Environment.MachineName));
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                // Logging must never prevent the requested file operation.
            }
        }

        public DataTable LoadRecent(int maximumRows = 5000)
        {
            var table = new DataTable();
            using (var connection = Open())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT
    LogId AS [ID],
    datetime(OccurredUtc, 'localtime') AS [Date and Time],
    Operation,
    SourcePath AS [Source],
    TargetPath AS [Target],
    Details,
    CASE WasSuccessful WHEN 1 THEN 'Yes' ELSE 'No' END AS [Successful],
    ErrorMessage AS [Error],
    WindowsUser AS [User],
    ComputerName AS [Computer]
FROM OperationLog
ORDER BY LogId DESC
LIMIT @maximumRows;";
                command.Parameters.AddWithValue(
                    "@maximumRows", Math.Max(1, maximumRows));
                using (var adapter = new SQLiteDataAdapter(command))
                    adapter.Fill(table);
            }
            return table;
        }

        public DataTable LoadTimeline(DateTime localStart, DateTime localEnd)
        {
            var table = new DataTable();
            using (var connection = Open())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT
    LogId,
    OccurredUtc,
    Operation,
    SourcePath,
    TargetPath,
    Details,
    WasSuccessful,
    ErrorMessage
FROM OperationLog
WHERE OccurredUtc >= @startUtc AND OccurredUtc < @endUtc
ORDER BY OccurredUtc ASC, LogId ASC;";
                command.Parameters.AddWithValue(
                    "@startUtc", localStart.ToUniversalTime().ToString("o"));
                command.Parameters.AddWithValue(
                    "@endUtc", localEnd.ToUniversalTime().ToString("o"));
                using (var adapter = new SQLiteDataAdapter(command))
                    adapter.Fill(table);
            }
            return table;
        }

        private SQLiteConnection Open()
        {
            var connection = new SQLiteConnection(
                "Data Source=" + DatabasePath + ";Version=3;");
            connection.Open();
            return connection;
        }

        private static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value;
        }
    }
}
