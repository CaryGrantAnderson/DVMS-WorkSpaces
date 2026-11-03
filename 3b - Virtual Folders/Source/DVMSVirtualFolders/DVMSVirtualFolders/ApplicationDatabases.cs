using System;
using System.Data.SQLite;
using System.IO;

namespace DVMSVirtualFolders
{
    internal sealed class ApplicationDatabases
    {
        public string DirectoryPath { get; private set; }
        public string SettingsPath { get; private set; }
        public string LogPath { get; private set; }
        public string MetadataPath { get; private set; }

        public ApplicationDatabases()
        {
            DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB");
            Directory.CreateDirectory(DirectoryPath);
            SettingsPath = Path.Combine(DirectoryPath, "DVMSVirtualFoldersSettings.db");
            LogPath = Path.Combine(DirectoryPath, "DVMSVirtualFoldersLog.db");
            MetadataPath = Path.Combine(DirectoryPath, "DVMSVirtualFoldersMetadata.db");
        }

        public void Initialize()
        {
            Execute(SettingsPath, "CREATE TABLE IF NOT EXISTS Settings(SettingKey TEXT PRIMARY KEY,SettingValue TEXT NOT NULL);");
            Execute(LogPath, "CREATE TABLE IF NOT EXISTS OperationLog(LogId INTEGER PRIMARY KEY AUTOINCREMENT,OccurredUtc TEXT NOT NULL,Operation TEXT NOT NULL,Target TEXT NULL,Details TEXT NULL,Successful INTEGER NOT NULL,Error TEXT NULL);");
            Execute(MetadataPath, "CREATE TABLE IF NOT EXISTS VirtualMetadata(MetadataId INTEGER PRIMARY KEY AUTOINCREMENT,VirtualFolderId INTEGER NULL,VirtualItemId INTEGER NULL,Description TEXT NULL,Notes TEXT NULL,Tags TEXT NULL,CustomIcon TEXT NULL,ModifiedUtc TEXT NOT NULL);");
        }

        public void Log(string operation, string target = null, string details = null, bool successful = true, string error = null)
        {
            try
            {
                using (var connection = Open(LogPath)) using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO OperationLog(OccurredUtc,Operation,Target,Details,Successful,Error) VALUES(@time,@operation,@target,@details,@success,@error);";
                    command.Parameters.AddWithValue("@time", DateTime.UtcNow.ToString("o")); command.Parameters.AddWithValue("@operation", operation);
                    command.Parameters.AddWithValue("@target", (object)target ?? DBNull.Value); command.Parameters.AddWithValue("@details", (object)details ?? DBNull.Value);
                    command.Parameters.AddWithValue("@success", successful ? 1 : 0); command.Parameters.AddWithValue("@error", (object)error ?? DBNull.Value); command.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private static void Execute(string path, string sql) { using (var c = Open(path)) using (var command = c.CreateCommand()) { command.CommandText = sql; command.ExecuteNonQuery(); } }
        private static SQLiteConnection Open(string path) { var c = new SQLiteConnection("Data Source=" + path + ";Version=3;"); c.Open(); return c; }
    }
}
