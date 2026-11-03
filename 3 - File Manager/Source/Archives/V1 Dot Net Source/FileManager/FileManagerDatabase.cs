using Microsoft.Data.Sqlite;

namespace DVMS.FileManager;

internal sealed class FileManagerDatabase
{
    private readonly string _connectionString;

    public FileManagerDatabase()
    {
        DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DVMS", "FileManager");
        Directory.CreateDirectory(DataDirectory);
        DatabasePath = Path.Combine(DataDirectory, "file-manager.db");
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    public string DataDirectory { get; }
    public string DatabasePath { get; }

    public void Initialize()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            PRAGMA journal_mode = WAL;
            PRAGMA foreign_keys = ON;

            CREATE TABLE IF NOT EXISTS ItemMetadata (
                Path TEXT PRIMARY KEY COLLATE NOCASE,
                Note TEXT NOT NULL DEFAULT '',
                Description TEXT NOT NULL DEFAULT '',
                ColorName TEXT NOT NULL DEFAULT '',
                UpdatedAtUtc TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Favorites (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Path TEXT NOT NULL UNIQUE COLLATE NOCASE,
                SortOrder INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS RecentLocations (
                Path TEXT PRIMARY KEY COLLATE NOCASE,
                VisitedAtUtc TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Settings (
                SettingKey TEXT PRIMARY KEY,
                SettingValue TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public Dictionary<string, ItemMetadata> GetMetadata(IEnumerable<string> paths)
    {
        var result = new Dictionary<string, ItemMetadata>(StringComparer.OrdinalIgnoreCase);
        using var connection = Open();
        foreach (var batch in paths.Chunk(300))
        {
            var names = batch.Select((_, i) => $"$p{i}").ToArray();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT Path, Note, Description, ColorName FROM ItemMetadata WHERE Path IN ({string.Join(',', names)})";
            var values = batch.ToArray();
            for (var i = 0; i < values.Length; i++) command.Parameters.AddWithValue(names[i], values[i]);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var item = new ItemMetadata(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                result[item.Path] = item;
            }
        }
        return result;
    }

    public void SaveMetadata(ItemMetadata metadata)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO ItemMetadata(Path, Note, Description, ColorName, UpdatedAtUtc)
            VALUES($path, $note, $description, $color, $updated)
            ON CONFLICT(Path) DO UPDATE SET Note=$note, Description=$description,
                ColorName=$color, UpdatedAtUtc=$updated;
            """;
        command.Parameters.AddWithValue("$path", metadata.Path);
        command.Parameters.AddWithValue("$note", metadata.Note);
        command.Parameters.AddWithValue("$description", metadata.Description);
        command.Parameters.AddWithValue("$color", metadata.ColorName);
        command.Parameters.AddWithValue("$updated", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<FavoriteLocation> GetFavorites()
    {
        var result = new List<FavoriteLocation>();
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Path, SortOrder FROM Favorites ORDER BY SortOrder, Name";
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(new FavoriteLocation(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3)));
        return result;
    }

    public void AddFavorite(string path)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR IGNORE INTO Favorites(Name, Path, SortOrder) VALUES($name, $path, (SELECT COALESCE(MAX(SortOrder), -1) + 1 FROM Favorites))";
        command.Parameters.AddWithValue("$name", new DirectoryInfo(path).Name is { Length: > 0 } name ? name : path);
        command.Parameters.AddWithValue("$path", path);
        command.ExecuteNonQuery();
    }

    public void RemoveFavorite(long id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Favorites WHERE Id=$id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void RecordVisit(string path)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO RecentLocations(Path, VisitedAtUtc) VALUES($path, $visited)
            ON CONFLICT(Path) DO UPDATE SET VisitedAtUtc=$visited;
            DELETE FROM RecentLocations WHERE Path NOT IN
                (SELECT Path FROM RecentLocations ORDER BY VisitedAtUtc DESC LIMIT 30);
            """;
        command.Parameters.AddWithValue("$path", path);
        command.Parameters.AddWithValue("$visited", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    public string? GetSetting(string key)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT SettingValue FROM Settings WHERE SettingKey=$key";
        command.Parameters.AddWithValue("$key", key);
        return command.ExecuteScalar() as string;
    }

    public void SetSetting(string key, string value)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Settings(SettingKey, SettingValue) VALUES($key,$value) ON CONFLICT(SettingKey) DO UPDATE SET SettingValue=$value";
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
