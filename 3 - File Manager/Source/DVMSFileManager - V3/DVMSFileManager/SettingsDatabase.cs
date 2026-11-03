using DevExpress.Dialogs.Core.View;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace DVMSFileManager
{
    internal sealed class SettingsDatabase
    {
        private readonly string connectionString;

        public SettingsDatabase()
        {
            string directory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "DB");
            Directory.CreateDirectory(directory);
            DatabasePath = Path.Combine(
                directory, "DVMSFileManagerSettings.db");
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
CREATE TABLE IF NOT EXISTS AppSettings
(
    SettingKey     TEXT PRIMARY KEY,
    SettingType    TEXT NOT NULL,
    SettingValue   TEXT,
    ModifiedUtc    TEXT NOT NULL
);";
                command.ExecuteNonQuery();
            }
        }

        public ApplicationSettings Load(string defaultHomeFolder)
        {
            ApplicationSettings settings =
                ApplicationSettings.CreateDefaults(defaultHomeFolder);
            var values = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            using (SQLiteConnection connection = Open())
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT SettingKey, SettingValue FROM AppSettings;";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        values[reader.GetString(0)] =
                            reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                }
            }

            settings.HomeFolder = Get(values, "Navigation.HomeFolder", settings.HomeFolder);
            settings.RestoreLastFolder = GetBool(values, "Navigation.RestoreLastFolder", settings.RestoreLastFolder);
            settings.LastFolder = Get(values, "Navigation.LastFolder", settings.LastFolder);
            settings.MaximumRecentFolders = GetInt(values, "Navigation.MaximumRecentFolders", settings.MaximumRecentFolders);
            settings.DefaultView = GetEnum(values, "Files.DefaultView", settings.DefaultView);
            settings.ShowDescriptions = GetBool(values, "Files.ShowDescriptions", settings.ShowDescriptions);
            settings.ShowPreviewColumn = GetBool(values, "Files.ShowPreviewColumn", settings.ShowPreviewColumn);
            settings.CalculateFolderSizes = GetBool(values, "Files.CalculateFolderSizes", settings.CalculateFolderSizes);
            settings.FolderSizeScanConcurrency = GetInt(values, "Files.FolderSizeScanConcurrency", settings.FolderSizeScanConcurrency);
            settings.ShowEmptyRemovableDrives = GetBool(values, "Files.ShowEmptyRemovableDrives", settings.ShowEmptyRemovableDrives);
            settings.ColorizeSizes = GetBool(values, "SizeColors.Enabled", settings.ColorizeSizes);
            settings.SmallSizeUpperLimitMb = GetInt(values, "SizeColors.SmallUpperLimitMb", settings.SmallSizeUpperLimitMb);
            settings.MediumSizeUpperLimitMb = GetInt(values, "SizeColors.MediumUpperLimitMb", settings.MediumSizeUpperLimitMb);
            settings.SmallSizeColor = GetColor(values, "SizeColors.SmallColorArgb", settings.SmallSizeColor);
            settings.MediumSizeColor = GetColor(values, "SizeColors.MediumColorArgb", settings.MediumSizeColor);
            settings.LargeSizeColor = GetColor(values, "SizeColors.LargeColorArgb", settings.LargeSizeColor);
            settings.ThemeName = Get(values, "Appearance.Theme", settings.ThemeName);
            settings.PaletteName = Get(values, "Appearance.Palette", settings.PaletteName);
            settings.RestoreSavedLayout = GetBool(values, "Layout.RestoreSavedLayout", settings.RestoreSavedLayout);
            settings.ConfirmDelete = GetBool(values, "Application.ConfirmDelete", settings.ConfirmDelete);
            settings.SearchEverything = GetBool(values, "Search.Everything", settings.SearchEverything);
            settings.SearchWindowsIndex = GetBool(values, "Search.WindowsIndex", settings.SearchWindowsIndex);
            settings.SearchDvmsMetadata = GetBool(values, "Search.DvmsMetadata", settings.SearchDvmsMetadata);
            settings.SearchSubdirectories = GetBool(values, "Search.Subdirectories", settings.SearchSubdirectories);
            settings.EverythingCliPath = Get(values, "Search.EverythingCliPath", settings.EverythingCliPath);
            settings.Validate();
            return settings;
        }

        public void Save(ApplicationSettings settings)
        {
            settings.Validate();
            using (SQLiteConnection connection = Open())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                SaveValue(connection, transaction, "Navigation.HomeFolder", "String", settings.HomeFolder);
                SaveValue(connection, transaction, "Navigation.RestoreLastFolder", "Boolean", settings.RestoreLastFolder.ToString());
                SaveValue(connection, transaction, "Navigation.LastFolder", "String", settings.LastFolder);
                SaveValue(connection, transaction, "Navigation.MaximumRecentFolders", "Integer", settings.MaximumRecentFolders.ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "Files.DefaultView", "ViewMode", settings.DefaultView.ToString());
                SaveValue(connection, transaction, "Files.ShowDescriptions", "Boolean", settings.ShowDescriptions.ToString());
                SaveValue(connection, transaction, "Files.ShowPreviewColumn", "Boolean", settings.ShowPreviewColumn.ToString());
                SaveValue(connection, transaction, "Files.CalculateFolderSizes", "Boolean", settings.CalculateFolderSizes.ToString());
                SaveValue(connection, transaction, "Files.FolderSizeScanConcurrency", "Integer", settings.FolderSizeScanConcurrency.ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "Files.ShowEmptyRemovableDrives", "Boolean", settings.ShowEmptyRemovableDrives.ToString());
                SaveValue(connection, transaction, "SizeColors.Enabled", "Boolean", settings.ColorizeSizes.ToString());
                SaveValue(connection, transaction, "SizeColors.SmallUpperLimitMb", "Integer", settings.SmallSizeUpperLimitMb.ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "SizeColors.MediumUpperLimitMb", "Integer", settings.MediumSizeUpperLimitMb.ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "SizeColors.SmallColorArgb", "Color", settings.SmallSizeColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "SizeColors.MediumColorArgb", "Color", settings.MediumSizeColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "SizeColors.LargeColorArgb", "Color", settings.LargeSizeColor.ToArgb().ToString(CultureInfo.InvariantCulture));
                SaveValue(connection, transaction, "Appearance.Theme", "String", settings.ThemeName);
                SaveValue(connection, transaction, "Appearance.Palette", "String", settings.PaletteName);
                SaveValue(connection, transaction, "Layout.RestoreSavedLayout", "Boolean", settings.RestoreSavedLayout.ToString());
                SaveValue(connection, transaction, "Application.ConfirmDelete", "Boolean", settings.ConfirmDelete.ToString());
                SaveValue(connection, transaction, "Search.Everything", "Boolean", settings.SearchEverything.ToString());
                SaveValue(connection, transaction, "Search.WindowsIndex", "Boolean", settings.SearchWindowsIndex.ToString());
                SaveValue(connection, transaction, "Search.DvmsMetadata", "Boolean", settings.SearchDvmsMetadata.ToString());
                SaveValue(connection, transaction, "Search.Subdirectories", "Boolean", settings.SearchSubdirectories.ToString());
                SaveValue(connection, transaction, "Search.EverythingCliPath", "String", settings.EverythingCliPath);
                transaction.Commit();
            }
        }

        public void SaveLastFolder(string path)
        {
            using (SQLiteConnection connection = Open())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                SaveValue(connection, transaction, "Navigation.LastFolder", "String", path ?? string.Empty);
                transaction.Commit();
            }
        }

        private SQLiteConnection Open()
        {
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        private static void SaveValue(SQLiteConnection connection, SQLiteTransaction transaction, string key, string type, string value)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
INSERT OR REPLACE INTO AppSettings
(SettingKey, SettingType, SettingValue, ModifiedUtc)
VALUES (@key, @type, @value, @modifiedUtc);";
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@value", value ?? string.Empty);
                command.Parameters.AddWithValue("@modifiedUtc", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
                command.ExecuteNonQuery();
            }
        }

        private static string Get(IDictionary<string, string> values, string key, string fallback)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : fallback;
        }

        private static bool GetBool(IDictionary<string, string> values, string key, bool fallback)
        {
            bool value;
            return bool.TryParse(Get(values, key, null), out value) ? value : fallback;
        }

        private static int GetInt(IDictionary<string, string> values, string key, int fallback)
        {
            int value;
            return int.TryParse(Get(values, key, null), NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : fallback;
        }

        private static T GetEnum<T>(IDictionary<string, string> values, string key, T fallback) where T : struct
        {
            T value;
            return Enum.TryParse(Get(values, key, null), true, out value) ? value : fallback;
        }

        private static Color GetColor(
            IDictionary<string, string> values,
            string key,
            Color fallback)
        {
            int argb;
            return int.TryParse(
                Get(values, key, null),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out argb)
                ? Color.FromArgb(argb)
                : fallback;
        }
    }
}
