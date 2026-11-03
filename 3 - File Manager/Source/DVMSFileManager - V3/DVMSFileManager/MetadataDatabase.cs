using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace DVMSFileManager
{
    internal sealed class FileIdentityKey
    {
        public string VolumeSerialNumber { get; set; }
        public string WindowsFileId { get; set; }
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }

    internal sealed class MetadataDatabase
    {
        private readonly string connectionString;

        public MetadataDatabase()
        {
            string directory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "DB");

            Directory.CreateDirectory(directory);
            DatabasePath = Path.Combine(directory, "DVMSFileMetadata.db");
            connectionString = "Data Source=" + DatabasePath +
                ";Version=3;Foreign Keys=True;";
        }

        public string DatabasePath { get; }

        public void Initialize()
        {
            using (SQLiteConnection connection = Open())
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
PRAGMA journal_mode=WAL;
PRAGMA foreign_keys=ON;

CREATE TABLE IF NOT EXISTS Files
(
    FileKey             TEXT PRIMARY KEY,
    VolumeSerialNumber  TEXT,
    WindowsFileId       TEXT,
    FullPath            TEXT NOT NULL COLLATE NOCASE,
    FileName            TEXT NOT NULL,
    LastSeenUtc         TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Files_WindowsIdentity
ON Files (VolumeSerialNumber, WindowsFileId)
WHERE VolumeSerialNumber IS NOT NULL
  AND WindowsFileId IS NOT NULL;

CREATE INDEX IF NOT EXISTS IX_Files_FullPath
ON Files (FullPath COLLATE NOCASE);

CREATE TABLE IF NOT EXISTS FileMetadata
(
    FileKey             TEXT PRIMARY KEY,
    DisplayName         TEXT NOT NULL DEFAULT '',
    Description         TEXT NOT NULL DEFAULT '',
    IsFavorite          INTEGER NOT NULL DEFAULT 0,
    Rating              INTEGER NOT NULL DEFAULT 0,
    Status              TEXT NOT NULL DEFAULT '',
    Workspace           TEXT NOT NULL DEFAULT '',
    CustomIconPath      TEXT NOT NULL DEFAULT '',
    FileNameColorArgb   INTEGER,
    IsFileNameBold      INTEGER NOT NULL DEFAULT 0,
    SourceUrl           TEXT NOT NULL DEFAULT '',
    CreatedUtc          TEXT NOT NULL,
    ModifiedUtc         TEXT NOT NULL,
    FOREIGN KEY (FileKey) REFERENCES Files(FileKey) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Tags
(
    TagKey      TEXT PRIMARY KEY,
    TagName     TEXT NOT NULL COLLATE NOCASE UNIQUE
);

CREATE TABLE IF NOT EXISTS FileTags
(
    FileKey     TEXT NOT NULL,
    TagKey      TEXT NOT NULL,
    PRIMARY KEY (FileKey, TagKey),
    FOREIGN KEY (FileKey) REFERENCES Files(FileKey) ON DELETE CASCADE,
    FOREIGN KEY (TagKey) REFERENCES Tags(TagKey) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS FileNotes
(
    NoteKey     TEXT PRIMARY KEY,
    FileKey     TEXT NOT NULL,
    Title       TEXT NOT NULL DEFAULT '',
    NoteText    TEXT NOT NULL DEFAULT '',
    IsPinned    INTEGER NOT NULL DEFAULT 0,
    SortOrder   INTEGER NOT NULL DEFAULT 0,
    CreatedUtc  TEXT NOT NULL,
    ModifiedUtc TEXT NOT NULL,
    FOREIGN KEY (FileKey) REFERENCES Files(FileKey) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_FileNotes_FileKey
ON FileNotes(FileKey, IsPinned DESC, SortOrder, ModifiedUtc DESC);";
                command.ExecuteNonQuery();
            }
        }

        public FileMetadataViewModel LoadOrCreate(
            FileIdentityKey identity,
            Action<FileMetadataViewModel> editNotes)
        {
            using (SQLiteConnection connection = Open())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                string fileKey = FindFileKey(connection, transaction, identity);
                DateTime now = DateTime.UtcNow;

                if (string.IsNullOrEmpty(fileKey))
                {
                    fileKey = Guid.NewGuid().ToString("D");
                    Execute(connection, transaction, @"
INSERT INTO Files
(FileKey, VolumeSerialNumber, WindowsFileId, FullPath, FileName, LastSeenUtc)
VALUES
(@FileKey, @VolumeSerialNumber, @WindowsFileId, @FullPath, @FileName, @LastSeenUtc);",
                        new SQLiteParameter("@FileKey", fileKey),
                        DbValue("@VolumeSerialNumber", identity.VolumeSerialNumber),
                        DbValue("@WindowsFileId", identity.WindowsFileId),
                        new SQLiteParameter("@FullPath", identity.FullPath),
                        new SQLiteParameter("@FileName", identity.FileName),
                        new SQLiteParameter("@LastSeenUtc", ToDbDate(now)));
                }
                else
                {
                    Execute(connection, transaction, @"
UPDATE Files SET
    VolumeSerialNumber = COALESCE(@VolumeSerialNumber, VolumeSerialNumber),
    WindowsFileId = COALESCE(@WindowsFileId, WindowsFileId),
    FullPath = @FullPath,
    FileName = @FileName,
    LastSeenUtc = @LastSeenUtc
WHERE FileKey = @FileKey;",
                        new SQLiteParameter("@FileKey", fileKey),
                        DbValue("@VolumeSerialNumber", identity.VolumeSerialNumber),
                        DbValue("@WindowsFileId", identity.WindowsFileId),
                        new SQLiteParameter("@FullPath", identity.FullPath),
                        new SQLiteParameter("@FileName", identity.FileName),
                        new SQLiteParameter("@LastSeenUtc", ToDbDate(now)));
                }

                FileMetadataViewModel result = LoadMetadata(
                    connection,
                    transaction,
                    fileKey,
                    identity.FullPath,
                    now,
                    editNotes);

                transaction.Commit();
                return result;
            }
        }

        public void SaveMetadata(FileMetadataViewModel metadata)
        {
            DateTime now = DateTime.UtcNow;
            DateTime created = metadata.MetadataCreatedUtc ?? now;

            using (SQLiteConnection connection = Open())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                Execute(connection, transaction, @"
INSERT INTO FileMetadata
(FileKey, DisplayName, Description, IsFavorite, Rating, Status,
 Workspace, CustomIconPath, FileNameColorArgb, IsFileNameBold,
 SourceUrl, CreatedUtc, ModifiedUtc)
VALUES
(@FileKey, @DisplayName, @Description, @IsFavorite, @Rating, @Status,
 @Workspace, @CustomIconPath, @FileNameColorArgb, @IsFileNameBold,
 @SourceUrl, @CreatedUtc, @ModifiedUtc)
ON CONFLICT(FileKey) DO UPDATE SET
 DisplayName=@DisplayName, Description=@Description,
 IsFavorite=@IsFavorite, Rating=@Rating, Status=@Status,
 Workspace=@Workspace, CustomIconPath=@CustomIconPath,
 FileNameColorArgb=@FileNameColorArgb,
 IsFileNameBold=@IsFileNameBold, SourceUrl=@SourceUrl,
 ModifiedUtc=@ModifiedUtc;",
                    new SQLiteParameter("@FileKey", metadata.FileKey),
                    new SQLiteParameter("@DisplayName", metadata.DisplayName ?? ""),
                    new SQLiteParameter("@Description", metadata.Description ?? ""),
                    new SQLiteParameter("@IsFavorite", metadata.IsFavorite ? 1 : 0),
                    new SQLiteParameter("@Rating", Math.Max(0, Math.Min(5, metadata.Rating))),
                    new SQLiteParameter("@Status", metadata.Status ?? ""),
                    new SQLiteParameter("@Workspace", metadata.Workspace ?? ""),
                    new SQLiteParameter("@CustomIconPath", metadata.CustomIconPath ?? ""),
                    DbValue("@FileNameColorArgb", metadata.GetColorArgb()),
                    new SQLiteParameter("@IsFileNameBold", metadata.IsFileNameBold ? 1 : 0),
                    new SQLiteParameter("@SourceUrl", metadata.SourceUrl ?? ""),
                    new SQLiteParameter("@CreatedUtc", ToDbDate(created)),
                    new SQLiteParameter("@ModifiedUtc", ToDbDate(now)));

                SaveTags(connection, transaction, metadata.FileKey, metadata.Tags);
                transaction.Commit();
            }

            metadata.MetadataCreatedUtc = created;
            metadata.MetadataModifiedUtc = now;
        }

        public void SaveNotes(string fileKey, IList<FileNote> notes)
        {
            using (SQLiteConnection connection = Open())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                Execute(connection, transaction,
                    "DELETE FROM FileNotes WHERE FileKey=@FileKey;",
                    new SQLiteParameter("@FileKey", fileKey));

                foreach (FileNote note in notes)
                {
                    Execute(connection, transaction, @"
INSERT INTO FileNotes
(NoteKey, FileKey, Title, NoteText, IsPinned, SortOrder,
 CreatedUtc, ModifiedUtc)
VALUES
(@NoteKey, @FileKey, @Title, @NoteText, @IsPinned, @SortOrder,
 @CreatedUtc, @ModifiedUtc);",
                        new SQLiteParameter("@NoteKey", note.NoteKey),
                        new SQLiteParameter("@FileKey", fileKey),
                        new SQLiteParameter("@Title", note.Title ?? ""),
                        new SQLiteParameter("@NoteText", note.NoteText ?? ""),
                        new SQLiteParameter("@IsPinned", note.IsPinned ? 1 : 0),
                        new SQLiteParameter("@SortOrder", note.SortOrder),
                        new SQLiteParameter("@CreatedUtc", ToDbDate(note.CreatedUtc)),
                        new SQLiteParameter("@ModifiedUtc", ToDbDate(note.ModifiedUtc)));
                }

                transaction.Commit();
            }
        }

        public int ReconcileKnownFiles(
            IEnumerable<FileIdentityKey> identities)
        {
            int updatedCount = 0;

            using (SQLiteConnection connection = Open())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
UPDATE Files SET
    FullPath=@FullPath,
    FileName=@FileName,
    LastSeenUtc=@LastSeenUtc
WHERE VolumeSerialNumber=@VolumeSerialNumber
  AND WindowsFileId=@WindowsFileId
  AND FullPath<>@FullPath COLLATE NOCASE;";

                SQLiteParameter volumeParameter =
                    command.Parameters.Add("@VolumeSerialNumber", DbType.String);
                SQLiteParameter fileIdParameter =
                    command.Parameters.Add("@WindowsFileId", DbType.String);
                SQLiteParameter pathParameter =
                    command.Parameters.Add("@FullPath", DbType.String);
                SQLiteParameter nameParameter =
                    command.Parameters.Add("@FileName", DbType.String);
                SQLiteParameter seenParameter =
                    command.Parameters.Add("@LastSeenUtc", DbType.String);

                foreach (FileIdentityKey identity in identities)
                {
                    if (string.IsNullOrEmpty(identity.VolumeSerialNumber) ||
                        string.IsNullOrEmpty(identity.WindowsFileId) ||
                        string.IsNullOrEmpty(identity.FullPath))
                    {
                        continue;
                    }

                    volumeParameter.Value = identity.VolumeSerialNumber;
                    fileIdParameter.Value = identity.WindowsFileId;
                    pathParameter.Value = identity.FullPath;
                    nameParameter.Value = identity.FileName ?? string.Empty;
                    seenParameter.Value = ToDbDate(DateTime.UtcNow);
                    updatedCount += command.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            return updatedCount;
        }

        public Dictionary<string, int> GetNoteCountsByPath()
        {
            var result = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

            using (SQLiteConnection connection = Open())
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT f.FullPath, COUNT(n.NoteKey)
FROM Files f
JOIN FileNotes n ON n.FileKey=f.FileKey
GROUP BY f.FileKey, f.FullPath;";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        result[reader.GetString(0)] = reader.GetInt32(1);
                }
            }

            return result;
        }

        public Dictionary<string, MetadataPresentation>
            GetPresentationByPath()
        {
            var result = new Dictionary<string, MetadataPresentation>(
                StringComparer.OrdinalIgnoreCase);

            using (SQLiteConnection connection = Open())
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT f.FullPath, m.CustomIconPath, m.FileNameColorArgb,
       m.IsFileNameBold
FROM Files f
JOIN FileMetadata m ON m.FileKey=f.FileKey
WHERE m.CustomIconPath<>''
   OR m.FileNameColorArgb IS NOT NULL
   OR m.IsFileNameBold<>0;";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetString(0)] =
                            new MetadataPresentation
                            {
                                CustomIconPath = reader.GetString(1),
                                FileNameColorArgb = reader.IsDBNull(2)
                                    ? (int?)null
                                    : reader.GetInt32(2),
                                IsFileNameBold = reader.GetInt32(3) != 0
                            };
                    }
                }
            }

            return result;
        }

        public Dictionary<string, string> GetDescriptionsByPath()
        {
            var result = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            using (SQLiteConnection connection = Open())
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT f.FullPath, m.Description
FROM Files f
JOIN FileMetadata m ON m.FileKey=f.FileKey
WHERE m.Description<>'';";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        result[reader.GetString(0)] = reader.GetString(1);
                }
            }

            return result;
        }

        private static FileMetadataViewModel LoadMetadata(
            SQLiteConnection connection,
            SQLiteTransaction transaction,
            string fileKey,
            string fullPath,
            DateTime lastSeen,
            Action<FileMetadataViewModel> editNotes)
        {
            var result = new FileMetadataViewModel(editNotes)
            {
                FileKey = fileKey,
                FullPath = fullPath,
                LastSeenUtc = lastSeen
            };

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
SELECT DisplayName, Description, IsFavorite, Rating, Status,
       Workspace, CustomIconPath, FileNameColorArgb,
       IsFileNameBold, SourceUrl, CreatedUtc, ModifiedUtc
FROM FileMetadata WHERE FileKey=@FileKey;";
                command.Parameters.AddWithValue("@FileKey", fileKey);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.DisplayName = reader.GetString(0);
                        result.Description = reader.GetString(1);
                        result.IsFavorite = reader.GetInt32(2) != 0;
                        result.Rating = reader.GetInt32(3);
                        result.Status = reader.GetString(4);
                        result.Workspace = reader.GetString(5);
                        result.CustomIconPath = reader.GetString(6);
                        result.SetColorArgb(reader.IsDBNull(7) ? null : reader.GetValue(7));
                        result.IsFileNameBold = reader.GetInt32(8) != 0;
                        result.SourceUrl = reader.GetString(9);
                        result.MetadataCreatedUtc = FromDbDate(reader.GetString(10));
                        result.MetadataModifiedUtc = FromDbDate(reader.GetString(11));
                    }
                }
            }

            result.Tags = LoadTags(connection, transaction, fileKey);
            result.NoteItems = LoadNotes(connection, transaction, fileKey);
            RefreshNotesSummary(result);
            return result;
        }

        internal static void RefreshNotesSummary(FileMetadataViewModel metadata)
        {
            metadata.Notes.Count = metadata.NoteItems.Count;
            FileNote latest = null;

            foreach (FileNote note in metadata.NoteItems)
            {
                if (latest == null || note.ModifiedUtc > latest.ModifiedUtc)
                    latest = note;
            }

            metadata.Notes.LatestTitle = latest?.Title ?? string.Empty;
            metadata.Notes.LastModifiedUtc = latest?.ModifiedUtc;
        }

        private static string FindFileKey(
            SQLiteConnection connection,
            SQLiteTransaction transaction,
            FileIdentityKey identity)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;

                if (!string.IsNullOrEmpty(identity.VolumeSerialNumber) &&
                    !string.IsNullOrEmpty(identity.WindowsFileId))
                {
                    command.CommandText = @"
SELECT FileKey FROM Files
WHERE VolumeSerialNumber=@VolumeSerialNumber
  AND WindowsFileId=@WindowsFileId LIMIT 1;";
                    command.Parameters.AddWithValue(
                        "@VolumeSerialNumber", identity.VolumeSerialNumber);
                    command.Parameters.AddWithValue(
                        "@WindowsFileId", identity.WindowsFileId);

                    object match = command.ExecuteScalar();
                    if (match != null && match != DBNull.Value)
                        return Convert.ToString(match, CultureInfo.InvariantCulture);

                    command.Parameters.Clear();
                }

                command.CommandText = @"
SELECT FileKey FROM Files
WHERE FullPath=@FullPath COLLATE NOCASE LIMIT 1;";
                command.Parameters.AddWithValue("@FullPath", identity.FullPath);
                object pathMatch = command.ExecuteScalar();
                return pathMatch == null || pathMatch == DBNull.Value
                    ? null
                    : Convert.ToString(pathMatch, CultureInfo.InvariantCulture);
            }
        }

        private static List<FileNote> LoadNotes(
            SQLiteConnection connection,
            SQLiteTransaction transaction,
            string fileKey)
        {
            var result = new List<FileNote>();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
SELECT NoteKey, Title, NoteText, IsPinned, SortOrder,
       CreatedUtc, ModifiedUtc
FROM FileNotes WHERE FileKey=@FileKey
ORDER BY IsPinned DESC, SortOrder, ModifiedUtc DESC;";
                command.Parameters.AddWithValue("@FileKey", fileKey);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new FileNote
                        {
                            NoteKey = reader.GetString(0),
                            Title = reader.GetString(1),
                            NoteText = reader.GetString(2),
                            IsPinned = reader.GetInt32(3) != 0,
                            SortOrder = reader.GetInt32(4),
                            CreatedUtc = FromDbDate(reader.GetString(5)),
                            ModifiedUtc = FromDbDate(reader.GetString(6))
                        });
                    }
                }
            }

            return result;
        }

        private static string LoadTags(
            SQLiteConnection connection,
            SQLiteTransaction transaction,
            string fileKey)
        {
            var tags = new List<string>();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
SELECT t.TagName FROM Tags t
JOIN FileTags ft ON ft.TagKey=t.TagKey
WHERE ft.FileKey=@FileKey ORDER BY t.TagName COLLATE NOCASE;";
                command.Parameters.AddWithValue("@FileKey", fileKey);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        tags.Add(reader.GetString(0));
                }
            }

            return string.Join(", ", tags);
        }

        private static void SaveTags(
            SQLiteConnection connection,
            SQLiteTransaction transaction,
            string fileKey,
            string tagsText)
        {
            Execute(connection, transaction,
                "DELETE FROM FileTags WHERE FileKey=@FileKey;",
                new SQLiteParameter("@FileKey", fileKey));

            var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string rawTag in (tagsText ?? "").Split(','))
            {
                string tag = rawTag.Trim();
                if (tag.Length == 0 || !unique.Add(tag))
                    continue;

                string tagKey = Guid.NewGuid().ToString("D");
                Execute(connection, transaction, @"
INSERT OR IGNORE INTO Tags(TagKey, TagName) VALUES(@TagKey, @TagName);",
                    new SQLiteParameter("@TagKey", tagKey),
                    new SQLiteParameter("@TagName", tag));

                Execute(connection, transaction, @"
INSERT OR IGNORE INTO FileTags(FileKey, TagKey)
SELECT @FileKey, TagKey FROM Tags WHERE TagName=@TagName COLLATE NOCASE;",
                    new SQLiteParameter("@FileKey", fileKey),
                    new SQLiteParameter("@TagName", tag));
            }
        }

        private SQLiteConnection Open()
        {
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys=ON;";
                command.ExecuteNonQuery();
            }
            return connection;
        }

        private static void Execute(
            SQLiteConnection connection,
            SQLiteTransaction transaction,
            string commandText,
            params SQLiteParameter[] parameters)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }

        private static SQLiteParameter DbValue(string name, object value)
        {
            return new SQLiteParameter(name, value ?? DBNull.Value);
        }

        private static string ToDbDate(DateTime value)
        {
            return value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
        }

        private static DateTime FromDbDate(string value)
        {
            return DateTime.Parse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);
        }
    }
}
