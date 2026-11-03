using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace DVMS.ButtonBar;

internal sealed class ButtonBarDatabase
{
    private readonly string _connectionString;
    private readonly bool _isHexButtonBar;

    public ButtonBarDatabase(string applicationName = "ButtonBar")
    {
        _isHexButtonBar = applicationName.Equals("HexButtonBar", StringComparison.OrdinalIgnoreCase);
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DVMS",
            applicationName);
        Directory.CreateDirectory(directory);
        DatabasePath = Path.Combine(directory, "button-bar.db");
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    public string DatabasePath { get; }

    public void Initialize()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Settings (
                SettingKey TEXT PRIMARY KEY,
                SettingValue TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Buttons (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DisplayName TEXT NOT NULL,
                LaunchTarget TEXT NOT NULL,
                Arguments TEXT NOT NULL DEFAULT '',
                WorkingDirectory TEXT NOT NULL DEFAULT '',
                IconPath TEXT NOT NULL DEFAULT '',
                SoundFile TEXT NOT NULL DEFAULT '',
                MonitorFileActivity INTEGER NOT NULL DEFAULT 0,
                SortOrder INTEGER NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_Buttons_SortOrder ON Buttons(SortOrder);

            CREATE TABLE IF NOT EXISTS LaunchHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ButtonId INTEGER NULL,
                DisplayName TEXT NOT NULL,
                LaunchTarget TEXT NOT NULL,
                Arguments TEXT NOT NULL,
                LaunchedAtUtc TEXT NOT NULL,
                LaunchedAtLocal TEXT NOT NULL,
                ProcessId INTEGER NULL,
                EndedAtUtc TEXT NULL,
                EndedAtLocal TEXT NULL,
                DurationSeconds REAL NULL,
                TrackingStatus TEXT NOT NULL DEFAULT 'Unknown / untrackable',
                ExitCode INTEGER NULL,
                FileMonitoringStatus TEXT NOT NULL DEFAULT 'Not requested'
            );

            CREATE INDEX IF NOT EXISTS IX_LaunchHistory_LaunchedAtUtc
                ON LaunchHistory(LaunchedAtUtc DESC);

            CREATE TABLE IF NOT EXISTS FileActivity (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                LaunchHistoryId INTEGER NOT NULL,
                ProcessId INTEGER NOT NULL,
                ProcessName TEXT NOT NULL,
                FilePath TEXT NOT NULL,
                AccessType TEXT NOT NULL,
                OpenedAtUtc TEXT NOT NULL,
                OpenedAtLocal TEXT NOT NULL,
                ClosedAtUtc TEXT NULL,
                ClosedAtLocal TEXT NULL,
                DurationSeconds REAL NULL,
                TrackingStatus TEXT NOT NULL,
                FOREIGN KEY (LaunchHistoryId) REFERENCES LaunchHistory(Id)
            );

            CREATE INDEX IF NOT EXISTS IX_FileActivity_LaunchHistory
                ON FileActivity(LaunchHistoryId, OpenedAtUtc DESC);
            """;
        command.ExecuteNonQuery();
        if (_isHexButtonBar)
        {
            using var appearance = connection.CreateCommand();
            appearance.CommandText = """
                CREATE TABLE IF NOT EXISTS HexButtonAppearance (
                    ButtonId INTEGER PRIMARY KEY,
                    ShowIcon INTEGER NOT NULL DEFAULT 1,
                    UseGradient INTEGER NOT NULL DEFAULT 1,
                    StartColorArgb INTEGER NOT NULL,
                    EndColorArgb INTEGER NOT NULL,
                    GradientDirection TEXT NOT NULL DEFAULT 'Vertical',
                    ShowLabel INTEGER NOT NULL DEFAULT 1,
                    LabelFontSize REAL NOT NULL DEFAULT 8,
                    LabelBold INTEGER NOT NULL DEFAULT 0,
                    TextColorArgb INTEGER NULL,
                    FOREIGN KEY (ButtonId) REFERENCES Buttons(Id) ON DELETE CASCADE
                );
                """;
            appearance.ExecuteNonQuery();
        }
        EnsureColumn(connection, "Buttons", "SoundFile", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Buttons", "MonitorFileActivity", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "LaunchHistory", "ProcessId", "INTEGER NULL");
        EnsureColumn(connection, "LaunchHistory", "EndedAtUtc", "TEXT NULL");
        EnsureColumn(connection, "LaunchHistory", "EndedAtLocal", "TEXT NULL");
        EnsureColumn(connection, "LaunchHistory", "DurationSeconds", "REAL NULL");
        EnsureColumn(connection, "LaunchHistory", "TrackingStatus", "TEXT NOT NULL DEFAULT 'Unknown / untrackable'");
        EnsureColumn(connection, "LaunchHistory", "ExitCode", "INTEGER NULL");
        EnsureColumn(connection, "LaunchHistory", "FileMonitoringStatus", "TEXT NOT NULL DEFAULT 'Not requested'");
        SeedSamples(connection);
    }

    public IReadOnlyList<ButtonDefinition> LoadButtons()
    {
        var buttons = new List<ButtonDefinition>();
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, DisplayName, LaunchTarget, Arguments, WorkingDirectory, IconPath, SoundFile,
                   MonitorFileActivity, SortOrder
            FROM Buttons
            ORDER BY SortOrder, Id;
            """;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            buttons.Add(new ButtonDefinition(
                reader.GetInt64(0), reader.GetString(1), reader.GetString(2),
                reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6),
                reader.GetInt64(7) != 0, reader.GetInt32(8)));
        }

        return buttons;
    }

    public long AddButton(ButtonDefinition button)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Buttons
                (DisplayName, LaunchTarget, Arguments, WorkingDirectory, IconPath, SoundFile,
                 MonitorFileActivity, SortOrder)
            VALUES ($name, $target, $arguments, $working, $icon, $sound, $monitorFiles,
                COALESCE((SELECT MAX(SortOrder) + 1 FROM Buttons), 0));
            SELECT last_insert_rowid();
            """;
        AddButtonParameters(command, button);
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    public void UpdateButton(ButtonDefinition button)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Buttons SET
                DisplayName = $name,
                LaunchTarget = $target,
                Arguments = $arguments,
                WorkingDirectory = $working,
                IconPath = $icon,
                SoundFile = $sound,
                MonitorFileActivity = $monitorFiles
            WHERE Id = $id;
            """;
        AddButtonParameters(command, button);
        command.Parameters.AddWithValue("$id", button.Id);
        command.ExecuteNonQuery();
    }

    public void DeleteButton(long id)
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM Buttons WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
        NormalizeOrder(connection, transaction);
        transaction.Commit();
    }

    public void MoveButton(long id, int direction)
    {
        var buttons = LoadButtons().ToList();
        var oldIndex = buttons.FindIndex(x => x.Id == id);
        var newIndex = Math.Clamp(oldIndex + direction, 0, buttons.Count - 1);
        if (oldIndex < 0 || oldIndex == newIndex)
        {
            return;
        }

        (buttons[oldIndex], buttons[newIndex]) = (buttons[newIndex], buttons[oldIndex]);
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        for (var index = 0; index < buttons.Count; index++)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "UPDATE Buttons SET SortOrder = $sort WHERE Id = $id;";
            command.Parameters.AddWithValue("$sort", index);
            command.Parameters.AddWithValue("$id", buttons[index].Id);
            command.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public ButtonBarSettings LoadSettings() => new(
        ReadBool("LockButtonEditing", false),
        ReadBool("LockWindowResize", false),
        ReadBool("LockWindowMovement", false),
        ReadBool("AlwaysOnTop", false),
        ReadBool("ShowButtonLabels", true),
        ReadBool("ShowMenuBar", true),
        Enum.TryParse<ButtonSize>(ReadSetting("ButtonSize"), out var size) ? size : ButtonSize.Medium,
        ReadSetting("SoundDirectory") ?? string.Empty,
        ReadInt("WindowX"), ReadInt("WindowY"), ReadInt("WindowWidth"), ReadInt("WindowHeight"));

    public void SaveSetting(string key, object value) => WriteSetting(key, value.ToString() ?? string.Empty);

    public HexButtonAppearance LoadHexAppearance(long buttonId)
    {
        if (!_isHexButtonBar) return HexButtonAppearance.Default(buttonId);
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ShowIcon, UseGradient, StartColorArgb, EndColorArgb, GradientDirection,
                   ShowLabel, LabelFontSize, LabelBold, TextColorArgb
            FROM HexButtonAppearance WHERE ButtonId = $id;
            """;
        command.Parameters.AddWithValue("$id", buttonId);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return HexButtonAppearance.Default(buttonId);
        return new HexButtonAppearance(buttonId, reader.GetInt64(0) != 0, reader.GetInt64(1) != 0,
            reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4), reader.GetInt64(5) != 0,
            reader.GetFloat(6), reader.GetInt64(7) != 0, reader.IsDBNull(8) ? null : reader.GetInt32(8));
    }

    public void SaveHexAppearance(HexButtonAppearance value)
    {
        if (!_isHexButtonBar) return;
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO HexButtonAppearance
                (ButtonId, ShowIcon, UseGradient, StartColorArgb, EndColorArgb,
                 GradientDirection, ShowLabel, LabelFontSize, LabelBold, TextColorArgb)
            VALUES ($id, $icon, $gradient, $start, $end, $direction, $label, $font, $bold, $text)
            ON CONFLICT(ButtonId) DO UPDATE SET
                ShowIcon=excluded.ShowIcon, UseGradient=excluded.UseGradient,
                StartColorArgb=excluded.StartColorArgb, EndColorArgb=excluded.EndColorArgb,
                GradientDirection=excluded.GradientDirection, ShowLabel=excluded.ShowLabel,
                LabelFontSize=excluded.LabelFontSize, LabelBold=excluded.LabelBold,
                TextColorArgb=excluded.TextColorArgb;
            """;
        command.Parameters.AddWithValue("$id", value.ButtonId);
        command.Parameters.AddWithValue("$icon", value.ShowIcon ? 1 : 0);
        command.Parameters.AddWithValue("$gradient", value.UseGradient ? 1 : 0);
        command.Parameters.AddWithValue("$start", value.StartColorArgb);
        command.Parameters.AddWithValue("$end", value.EndColorArgb);
        command.Parameters.AddWithValue("$direction", value.GradientDirection);
        command.Parameters.AddWithValue("$label", value.ShowLabel ? 1 : 0);
        command.Parameters.AddWithValue("$font", value.LabelFontSize);
        command.Parameters.AddWithValue("$bold", value.LabelBold ? 1 : 0);
        command.Parameters.AddWithValue("$text", value.TextColorArgb is null ? DBNull.Value : value.TextColorArgb.Value);
        command.ExecuteNonQuery();
    }

    public long LogLaunch(ButtonDefinition button, Process? process)
    {
        var now = DateTimeOffset.Now;
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO LaunchHistory
                (ButtonId, DisplayName, LaunchTarget, Arguments, LaunchedAtUtc, LaunchedAtLocal,
                 ProcessId, TrackingStatus)
            VALUES
                ($buttonId, $name, $target, $arguments, $utc, $local, $processId, $status);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$buttonId", button.Id > 0 ? button.Id : DBNull.Value);
        command.Parameters.AddWithValue("$name", button.DisplayName);
        command.Parameters.AddWithValue("$target", button.LaunchTarget);
        command.Parameters.AddWithValue("$arguments", button.Arguments);
        command.Parameters.AddWithValue("$utc", now.UtcDateTime.ToString("O"));
        command.Parameters.AddWithValue("$local", now.ToString("O"));
        command.Parameters.AddWithValue("$processId", process is null ? DBNull.Value : process.Id);
        command.Parameters.AddWithValue("$status", process is null ? "Unknown / untrackable" : "Running");
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    public void CompleteLaunch(
        long historyId,
        DateTimeOffset endedAt,
        double durationSeconds,
        string trackingStatus,
        int? exitCode)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE LaunchHistory SET
                EndedAtUtc = $endedUtc,
                EndedAtLocal = $endedLocal,
                DurationSeconds = $duration,
                TrackingStatus = $status,
                ExitCode = $exitCode
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$endedUtc", endedAt.UtcDateTime.ToString("O"));
        command.Parameters.AddWithValue("$endedLocal", endedAt.ToString("O"));
        command.Parameters.AddWithValue("$duration", durationSeconds);
        command.Parameters.AddWithValue("$status", trackingStatus);
        command.Parameters.AddWithValue("$exitCode", exitCode is null ? DBNull.Value : exitCode.Value);
        command.Parameters.AddWithValue("$id", historyId);
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<LaunchHistoryEntry> LoadLaunchHistory(int maximumRows = 1000)
    {
        var entries = new List<LaunchHistoryEntry>();
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, ButtonId, DisplayName, LaunchTarget, Arguments, LaunchedAtLocal, LaunchedAtUtc,
                   ProcessId, EndedAtLocal, EndedAtUtc, DurationSeconds, TrackingStatus, ExitCode,
                   FileMonitoringStatus
            FROM LaunchHistory
            ORDER BY Id DESC
            LIMIT $maximumRows;
            """;
        command.Parameters.AddWithValue("$maximumRows", maximumRows);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(new LaunchHistoryEntry(
                reader.GetInt64(0),
                reader.IsDBNull(1) ? null : reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                DateTimeOffset.Parse(reader.GetString(5)),
                DateTime.Parse(reader.GetString(6), null, System.Globalization.DateTimeStyles.RoundtripKind),
                reader.IsDBNull(7) ? null : reader.GetInt32(7),
                reader.IsDBNull(8) ? null : DateTimeOffset.Parse(reader.GetString(8)),
                reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9), null, System.Globalization.DateTimeStyles.RoundtripKind),
                reader.IsDBNull(10) ? null : reader.GetDouble(10),
                reader.GetString(11),
                reader.IsDBNull(12) ? null : reader.GetInt32(12),
                reader.GetString(13)));
        }
        return entries;
    }

    public void UpdateFileMonitoringStatus(long historyId, string status)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE LaunchHistory SET FileMonitoringStatus = $status WHERE Id = $id;";
        command.Parameters.AddWithValue("$status", status);
        command.Parameters.AddWithValue("$id", historyId);
        command.ExecuteNonQuery();
    }

    public long OpenFileActivity(
        long historyId, int processId, string processName, string filePath, DateTimeOffset openedAt)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO FileActivity
                (LaunchHistoryId, ProcessId, ProcessName, FilePath, AccessType,
                 OpenedAtUtc, OpenedAtLocal, TrackingStatus)
            VALUES
                ($historyId, $processId, $processName, $filePath, 'Open',
                 $openedUtc, $openedLocal, 'Open');
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$historyId", historyId);
        command.Parameters.AddWithValue("$processId", processId);
        command.Parameters.AddWithValue("$processName", processName);
        command.Parameters.AddWithValue("$filePath", filePath);
        command.Parameters.AddWithValue("$openedUtc", openedAt.UtcDateTime.ToString("O"));
        command.Parameters.AddWithValue("$openedLocal", openedAt.ToString("O"));
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    public void CloseFileActivity(
        long activityId, DateTimeOffset openedAt, DateTimeOffset closedAt, string accessType)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE FileActivity SET
                AccessType = $accessType,
                ClosedAtUtc = $closedUtc,
                ClosedAtLocal = $closedLocal,
                DurationSeconds = $duration,
                TrackingStatus = 'Closed'
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$accessType", accessType);
        command.Parameters.AddWithValue("$closedUtc", closedAt.UtcDateTime.ToString("O"));
        command.Parameters.AddWithValue("$closedLocal", closedAt.ToString("O"));
        command.Parameters.AddWithValue("$duration", Math.Max(0, (closedAt - openedAt).TotalSeconds));
        command.Parameters.AddWithValue("$id", activityId);
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<FileActivityEntry> LoadFileActivity(int maximumRows = 2000)
    {
        var entries = new List<FileActivityEntry>();
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, LaunchHistoryId, ProcessId, ProcessName, FilePath, AccessType,
                   OpenedAtLocal, ClosedAtLocal, DurationSeconds, TrackingStatus
            FROM FileActivity
            ORDER BY Id DESC
            LIMIT $maximumRows;
            """;
        command.Parameters.AddWithValue("$maximumRows", maximumRows);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(new FileActivityEntry(
                reader.GetInt64(0), reader.GetInt64(1), reader.GetInt32(2), reader.GetString(3),
                reader.GetString(4), reader.GetString(5), DateTimeOffset.Parse(reader.GetString(6)),
                reader.IsDBNull(7) ? null : DateTimeOffset.Parse(reader.GetString(7)),
                reader.IsDBNull(8) ? null : reader.GetDouble(8), reader.GetString(9)));
        }
        return entries;
    }

    public void SaveWindowBounds(Rectangle bounds)
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        foreach (var pair in new Dictionary<string, int>
                 {
                     ["WindowX"] = bounds.X, ["WindowY"] = bounds.Y,
                     ["WindowWidth"] = bounds.Width, ["WindowHeight"] = bounds.Height
                 })
        {
            WriteSetting(connection, transaction, pair.Key, pair.Value.ToString());
        }
        transaction.Commit();
    }

    private static void SeedSamples(SqliteConnection connection)
    {
        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM Buttons;";
        if ((long)(count.ExecuteScalar() ?? 0L) != 0)
        {
            return;
        }

        var samples = new[]
        {
            new ButtonDefinition(0, "Calculator", "calc.exe", "", "", "", "", false, 0),
            new ButtonDefinition(0, "Notepad", "notepad.exe", "", "", "", "", false, 1),
            new ButtonDefinition(0, "Settings", "ms-settings:", "", "", "", "", false, 2)
        };
        foreach (var sample in samples)
        {
            using var insert = connection.CreateCommand();
            insert.CommandText = """
                INSERT INTO Buttons
                    (DisplayName, LaunchTarget, Arguments, WorkingDirectory, IconPath, SoundFile,
                     MonitorFileActivity, SortOrder)
                VALUES ($name, $target, '', '', '', '', 0, $sort);
                """;
            insert.Parameters.AddWithValue("$name", sample.DisplayName);
            insert.Parameters.AddWithValue("$target", sample.LaunchTarget);
            insert.Parameters.AddWithValue("$sort", sample.SortOrder);
            insert.ExecuteNonQuery();
        }
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private string? ReadSetting(string key)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT SettingValue FROM Settings WHERE SettingKey = $key;";
        command.Parameters.AddWithValue("$key", key);
        return command.ExecuteScalar() as string;
    }

    private bool ReadBool(string key, bool fallback) =>
        bool.TryParse(ReadSetting(key), out var value) ? value : fallback;

    private int? ReadInt(string key) =>
        int.TryParse(ReadSetting(key), out var value) ? value : null;

    private void WriteSetting(string key, string value)
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        WriteSetting(connection, transaction, key, value);
        transaction.Commit();
    }

    private static void WriteSetting(SqliteConnection connection, SqliteTransaction transaction, string key, string value)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO Settings (SettingKey, SettingValue) VALUES ($key, $value)
            ON CONFLICT(SettingKey) DO UPDATE SET SettingValue = excluded.SettingValue;
            """;
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }

    private static void AddButtonParameters(SqliteCommand command, ButtonDefinition button)
    {
        command.Parameters.AddWithValue("$name", button.DisplayName);
        command.Parameters.AddWithValue("$target", button.LaunchTarget);
        command.Parameters.AddWithValue("$arguments", button.Arguments);
        command.Parameters.AddWithValue("$working", button.WorkingDirectory);
        command.Parameters.AddWithValue("$icon", button.IconPath);
        command.Parameters.AddWithValue("$sound", button.SoundFile);
        command.Parameters.AddWithValue("$monitorFiles", button.MonitorFileActivity ? 1 : 0);
    }

    private static void EnsureColumn(SqliteConnection connection, string table, string column, string definition)
    {
        using var inspect = connection.CreateCommand();
        inspect.CommandText = $"PRAGMA table_info({table});";
        using var reader = inspect.ExecuteReader();
        while (reader.Read())
        {
            if (reader.GetString(1).Equals(column, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }
        reader.Close();

        using var alter = connection.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition};";
        alter.ExecuteNonQuery();
    }

    private static void NormalizeOrder(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            WITH Ordered AS (SELECT Id, ROW_NUMBER() OVER (ORDER BY SortOrder, Id) - 1 AS NewSort FROM Buttons)
            UPDATE Buttons SET SortOrder = (SELECT NewSort FROM Ordered WHERE Ordered.Id = Buttons.Id);
            """;
        command.ExecuteNonQuery();
    }
}
