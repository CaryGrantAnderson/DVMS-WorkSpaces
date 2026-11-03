using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace DVMSVirtualFolders
{
    internal sealed class VirtualFolderRecord
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    internal sealed class VirtualItemRecord
    {
        public long Id { get; set; }
        public long FolderId { get; set; }
        public string DisplayName { get; set; }
        public string TargetPath { get; set; }
        public bool IsFolder { get; set; }
        public bool Exists { get { return IsFolder
            ? Directory.Exists(TargetPath) : File.Exists(TargetPath); } }
        public string SourceFolder { get { return IsFolder
            ? Path.GetDirectoryName(TargetPath.TrimEnd('\\'))
            : Path.GetDirectoryName(TargetPath); } }
    }

    internal sealed class VirtualFoldersDatabase
    {
        public string DatabasePath { get; private set; }

        public VirtualFoldersDatabase()
        {
            string directory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "DB");
            Directory.CreateDirectory(directory);
            DatabasePath = Path.Combine(directory, "DVMSVirtualFolders.db");
        }

        public void Initialize()
        {
            using (var connection = Open())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
PRAGMA foreign_keys=ON;
CREATE TABLE IF NOT EXISTS VirtualFolders (
    VirtualFolderId INTEGER PRIMARY KEY AUTOINCREMENT,
    ParentVirtualFolderId INTEGER NULL,
    Name TEXT NOT NULL,
    Description TEXT NULL,
    SortOrder INTEGER NOT NULL DEFAULT 0,
    CreatedUtc TEXT NOT NULL,
    ModifiedUtc TEXT NOT NULL,
    FOREIGN KEY (ParentVirtualFolderId) REFERENCES VirtualFolders(VirtualFolderId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_VirtualFolders_Parent
    ON VirtualFolders(ParentVirtualFolderId, SortOrder, Name);
CREATE TABLE IF NOT EXISTS VirtualItems (
    VirtualItemId INTEGER PRIMARY KEY AUTOINCREMENT,
    VirtualFolderId INTEGER NOT NULL,
    ItemType TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    TargetPath TEXT NOT NULL,
    VolumeId TEXT NULL,
    FileId TEXT NULL,
    SortOrder INTEGER NOT NULL DEFAULT 0,
    CreatedUtc TEXT NOT NULL,
    FOREIGN KEY (VirtualFolderId) REFERENCES VirtualFolders(VirtualFolderId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_VirtualItems_Folder
    ON VirtualItems(VirtualFolderId, SortOrder, DisplayName);";
                command.ExecuteNonQuery();
            }
        }

        public List<VirtualFolderRecord> LoadFolders()
        {
            var result = new List<VirtualFolderRecord>();
            using (var connection = Open())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT VirtualFolderId, ParentVirtualFolderId, Name, SortOrder FROM VirtualFolders ORDER BY SortOrder, Name COLLATE NOCASE;";
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) result.Add(new VirtualFolderRecord
                    {
                        Id = reader.GetInt64(0),
                        ParentId = reader.IsDBNull(1) ? (long?)null : reader.GetInt64(1),
                        Name = reader.GetString(2),
                        SortOrder = reader.GetInt32(3)
                    });
            }
            return result;
        }

        public List<VirtualItemRecord> LoadItems(long folderId)
        {
            var result = new List<VirtualItemRecord>();
            using (var connection = Open())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT VirtualItemId, VirtualFolderId, DisplayName, TargetPath, ItemType FROM VirtualItems WHERE VirtualFolderId=@folder ORDER BY SortOrder, DisplayName COLLATE NOCASE;";
                command.Parameters.AddWithValue("@folder", folderId);
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) result.Add(new VirtualItemRecord
                    {
                        Id = reader.GetInt64(0), FolderId = reader.GetInt64(1),
                        DisplayName = reader.GetString(2), TargetPath = reader.GetString(3),
                        IsFolder = string.Equals(reader.GetString(4), "Folder", StringComparison.OrdinalIgnoreCase)
                    });
            }
            return result;
        }

        public long AddFolder(long? parentId, string name)
        {
            using (var connection = Open()) using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO VirtualFolders(ParentVirtualFolderId,Name,SortOrder,CreatedUtc,ModifiedUtc) SELECT @parent,@name,COALESCE(MAX(SortOrder),-1)+1,@now,@now FROM VirtualFolders WHERE (ParentVirtualFolderId=@parent OR (ParentVirtualFolderId IS NULL AND @parent IS NULL)); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@parent", parentId.HasValue ? (object)parentId.Value : DBNull.Value);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
                return Convert.ToInt64(command.ExecuteScalar());
            }
        }

        public void RenameFolder(long id, string name)
        {
            Execute("UPDATE VirtualFolders SET Name=@value,ModifiedUtc=@now WHERE VirtualFolderId=@id;", id, name);
        }

        public void DeleteFolder(long id)
        {
            Execute("DELETE FROM VirtualFolders WHERE VirtualFolderId=@id;", id, null);
        }

        public Dictionary<long, int[]> LoadDirectCounts()
        {
            var result = new Dictionary<long, int[]>();
            foreach (var folder in LoadFolders()) result[folder.Id] = new int[3];
            using (var connection = Open())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ParentVirtualFolderId,COUNT(*) FROM VirtualFolders WHERE ParentVirtualFolderId IS NOT NULL GROUP BY ParentVirtualFolderId;";
                    using (var reader = command.ExecuteReader()) while (reader.Read()) if (result.ContainsKey(reader.GetInt64(0))) result[reader.GetInt64(0)][0] = reader.GetInt32(1);
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT VirtualFolderId,ItemType,COUNT(*) FROM VirtualItems GROUP BY VirtualFolderId,ItemType;";
                    using (var reader = command.ExecuteReader()) while (reader.Read())
                    {
                        long id = reader.GetInt64(0); if (!result.ContainsKey(id)) continue;
                        result[id][string.Equals(reader.GetString(1), "Folder", StringComparison.OrdinalIgnoreCase) ? 1 : 2] = reader.GetInt32(2);
                    }
                }
            }
            return result;
        }

        public void MoveFolder(long folderId, long? newParentId, int newIndex)
        {
            List<VirtualFolderRecord> folders = LoadFolders();
            var moving = folders.First(x => x.Id == folderId);
            if (newParentId == folderId || IsDescendant(folders, newParentId, folderId))
                throw new InvalidOperationException("A virtual folder cannot be moved inside itself or one of its subfolders.");
            var siblings = folders.Where(x => x.ParentId == newParentId && x.Id != folderId).OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            newIndex = Math.Max(0, Math.Min(newIndex, siblings.Count));
            siblings.Insert(newIndex, moving);
            using (var connection = Open()) using (var transaction = connection.BeginTransaction())
            {
                for (int index = 0; index < siblings.Count; index++)
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = "UPDATE VirtualFolders SET ParentVirtualFolderId=@parent,SortOrder=@sort,ModifiedUtc=@now WHERE VirtualFolderId=@id;";
                    command.Parameters.AddWithValue("@parent", newParentId.HasValue ? (object)newParentId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@sort", index);
                    command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
                    command.Parameters.AddWithValue("@id", siblings[index].Id);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private static bool IsDescendant(List<VirtualFolderRecord> folders, long? possibleDescendant, long folderId)
        {
            while (possibleDescendant.HasValue)
            {
                if (possibleDescendant.Value == folderId) return true;
                var folder = folders.FirstOrDefault(x => x.Id == possibleDescendant.Value);
                possibleDescendant = folder == null ? null : folder.ParentId;
            }
            return false;
        }

        public void AddItem(long folderId, string path)
        {
            bool isFolder = Directory.Exists(path);
            string name = Path.GetFileName(path.TrimEnd('\\', '/'));
            if (string.IsNullOrWhiteSpace(name)) name = path;
            using (var connection = Open()) using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO VirtualItems(VirtualFolderId,ItemType,DisplayName,TargetPath,CreatedUtc) VALUES(@folder,@type,@name,@path,@now);";
                command.Parameters.AddWithValue("@folder", folderId);
                command.Parameters.AddWithValue("@type", isFolder ? "Folder" : "File");
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@path", path);
                command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        public void DeleteItem(long id)
        {
            Execute("DELETE FROM VirtualItems WHERE VirtualItemId=@id;", id, null);
        }

        private void Execute(string sql, long id, string value)
        {
            using (var connection = Open()) using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@value", (object)value ?? DBNull.Value);
                command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        private SQLiteConnection Open()
        {
            var connection = new SQLiteConnection("Data Source=" + DatabasePath + ";Version=3;");
            connection.Open();
            using (var command = connection.CreateCommand()) { command.CommandText = "PRAGMA foreign_keys=ON;"; command.ExecuteNonQuery(); }
            return connection;
        }
    }
}
