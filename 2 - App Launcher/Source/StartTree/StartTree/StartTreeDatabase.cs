using Microsoft.Data.Sqlite;

namespace DVMS.StartTree;

internal sealed class StartTreeDatabase
{
    private readonly string _connectionString;

    public StartTreeDatabase()
    {
        var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DVMS",
            "StartTree");
        Directory.CreateDirectory(dataDirectory);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(dataDirectory, "start-tree.db"),
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    public void Initialize()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Settings (
                SettingKey TEXT PRIMARY KEY,
                SettingValue TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS MenuItems (
                ItemKey TEXT PRIMARY KEY,
                ParentKey TEXT NULL,
                DisplayName TEXT NOT NULL,
                LaunchPath TEXT NOT NULL,
                Source TEXT NOT NULL,
                IsFolder INTEGER NOT NULL,
                SortOrder INTEGER NOT NULL,
                FOREIGN KEY (ParentKey) REFERENCES MenuItems(ItemKey)
            );

            CREATE INDEX IF NOT EXISTS IX_MenuItems_ParentKey
                ON MenuItems(ParentKey, SortOrder, DisplayName);
            """;
        command.ExecuteNonQuery();
    }

    public AppSettings LoadSettings() => new(
        ReadBool("KeepMenuOpenAfterLaunch", false),
        ReadInt("IconX"),
        ReadInt("IconY"));

    public void SaveKeepMenuOpenAfterLaunch(bool value) =>
        WriteSetting("KeepMenuOpenAfterLaunch", value.ToString());

    public void SaveIconLocation(Point location)
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        WriteSetting(connection, transaction, "IconX", location.X.ToString());
        WriteSetting(connection, transaction, "IconY", location.Y.ToString());
        transaction.Commit();
    }

    public void ReplaceMenuItems(IReadOnlyCollection<MenuItemDefinition> items)
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();

        using (var delete = connection.CreateCommand())
        {
            delete.Transaction = transaction;
            delete.CommandText = "DELETE FROM MenuItems;";
            delete.ExecuteNonQuery();
        }

        foreach (var item in items.OrderBy(x => x.ParentKey is not null))
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText = """
                INSERT INTO MenuItems
                    (ItemKey, ParentKey, DisplayName, LaunchPath, Source, IsFolder, SortOrder)
                VALUES
                    ($key, $parent, $name, $path, $source, $folder, $sort);
                """;
            insert.Parameters.AddWithValue("$key", item.ItemKey);
            insert.Parameters.AddWithValue("$parent", (object?)item.ParentKey ?? DBNull.Value);
            insert.Parameters.AddWithValue("$name", item.DisplayName);
            insert.Parameters.AddWithValue("$path", item.LaunchPath);
            insert.Parameters.AddWithValue("$source", item.Source);
            insert.Parameters.AddWithValue("$folder", item.IsFolder ? 1 : 0);
            insert.Parameters.AddWithValue("$sort", item.SortOrder);
            insert.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public IReadOnlyList<MenuItemDefinition> LoadMenuItems()
    {
        var results = new List<MenuItemDefinition>();
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ItemKey, ParentKey, DisplayName, LaunchPath, Source, IsFolder, SortOrder
            FROM MenuItems
            ORDER BY ParentKey, SortOrder, DisplayName COLLATE NOCASE;
            """;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new MenuItemDefinition(
                reader.GetString(0),
                reader.IsDBNull(1) ? null : reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetInt64(5) != 0,
                reader.GetInt32(6)));
        }

        return results;
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private bool ReadBool(string key, bool fallback) =>
        bool.TryParse(ReadSetting(key), out var value) ? value : fallback;

    private int? ReadInt(string key) =>
        int.TryParse(ReadSetting(key), out var value) ? value : null;

    private string? ReadSetting(string key)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT SettingValue FROM Settings WHERE SettingKey = $key;";
        command.Parameters.AddWithValue("$key", key);
        return command.ExecuteScalar() as string;
    }

    private void WriteSetting(string key, string value)
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        WriteSetting(connection, transaction, key, value);
        transaction.Commit();
    }

    private static void WriteSetting(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string key,
        string value)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO Settings (SettingKey, SettingValue)
            VALUES ($key, $value)
            ON CONFLICT(SettingKey) DO UPDATE SET SettingValue = excluded.SettingValue;
            """;
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }
}
