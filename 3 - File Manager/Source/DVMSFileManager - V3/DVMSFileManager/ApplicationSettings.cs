using DevExpress.Dialogs.Core.View;
using System;
using System.ComponentModel;
using System.Drawing;

namespace DVMSFileManager
{
    internal sealed class ApplicationSettings
    {
        [Category("Navigation"), DisplayName("Home folder")]
        [Description("Folder opened by the Home navigation command.")]
        public string HomeFolder { get; set; }

        [Category("Navigation"), DisplayName("Restore last folder")]
        public bool RestoreLastFolder { get; set; }

        [Browsable(false)]
        public string LastFolder { get; set; }

        [Category("Navigation"), DisplayName("Maximum recent folders")]
        public int MaximumRecentFolders { get; set; }

        [Category("Files and Folders"), DisplayName("Default view")]
        public ViewMode DefaultView { get; set; }

        [Category("Files and Folders"), DisplayName("Show descriptions")]
        public bool ShowDescriptions { get; set; }

        [Category("Files and Folders"), DisplayName("Show Preview column")]
        [Description("Displays metadata descriptions in a Preview column in Details view.")]
        public bool ShowPreviewColumn { get; set; }

        [Category("Files and Folders"), DisplayName("Calculate folder sizes")]
        public bool CalculateFolderSizes { get; set; }

        [Category("Files and Folders"), DisplayName("Folder scan concurrency")]
        [Description("Number of folder-size scans allowed at the same time (1-8).")]
        public int FolderSizeScanConcurrency { get; set; }

        [Category("Files and Folders"), DisplayName("Show empty removable drives")]
        [Description("When enabled, empty card-reader and removable-drive slots remain visible and dimmed.")]
        public bool ShowEmptyRemovableDrives { get; set; }

        [Category("Size Colors"), DisplayName("Colorize file and folder sizes")]
        public bool ColorizeSizes { get; set; }

        [Category("Size Colors"), DisplayName("Small-size upper limit (MB)")]
        public int SmallSizeUpperLimitMb { get; set; }

        [Category("Size Colors"), DisplayName("Medium-size upper limit (MB)")]
        public int MediumSizeUpperLimitMb { get; set; }

        [Category("Size Colors"), DisplayName("Small-size color")]
        public Color SmallSizeColor { get; set; }

        [Category("Size Colors"), DisplayName("Medium-size color")]
        public Color MediumSizeColor { get; set; }

        [Category("Size Colors"), DisplayName("Large-size color")]
        public Color LargeSizeColor { get; set; }

        [Category("Appearance"), DisplayName("DevExpress theme")]
        [Description("Examples: Office 2019 Colorful, The Bezier, WXI.")]
        public string ThemeName { get; set; }

        [Browsable(false)]
        public string PaletteName { get; set; }

        [Category("Panels and Layout"), DisplayName("Restore saved layout")]
        public bool RestoreSavedLayout { get; set; }

        [Category("Application"), DisplayName("Confirm delete")]
        public bool ConfirmDelete { get; set; }

        [Category("Search"), DisplayName("Enable Everything search")]
        public bool SearchEverything { get; set; }
        [Category("Search"), DisplayName("Enable Windows Index search")]
        public bool SearchWindowsIndex { get; set; }
        [Category("Search"), DisplayName("Enable DVMS metadata search")]
        public bool SearchDvmsMetadata { get; set; }
        [Category("Search"), DisplayName("Search subdirectories")]
        [Description("Includes all folders below each selected search location.")]
        public bool SearchSubdirectories { get; set; }
        [Category("Search"), DisplayName("Everything es.exe path")]
        public string EverythingCliPath { get; set; }

        public static ApplicationSettings CreateDefaults(string homeFolder)
        {
            return new ApplicationSettings
            {
                HomeFolder = string.IsNullOrWhiteSpace(homeFolder)
                    ? "C:\\"
                    : homeFolder,
                RestoreLastFolder = false,
                LastFolder = string.Empty,
                MaximumRecentFolders = 20,
                DefaultView = ViewMode.Details,
                ShowDescriptions = false,
                ShowPreviewColumn = false,
                CalculateFolderSizes = true,
                FolderSizeScanConcurrency = 2,
                ShowEmptyRemovableDrives = true,
                ColorizeSizes = true,
                SmallSizeUpperLimitMb = 10,
                MediumSizeUpperLimitMb = 1024,
                SmallSizeColor = Color.SeaGreen,
                MediumSizeColor = Color.DarkOrange,
                LargeSizeColor = Color.Firebrick,
                ThemeName = string.Empty,
                PaletteName = string.Empty,
                RestoreSavedLayout = true,
                ConfirmDelete = true
                ,SearchEverything = true
                ,SearchWindowsIndex = true
                ,SearchDvmsMetadata = true
                ,SearchSubdirectories = true
                ,EverythingCliPath = string.Empty
            };
        }

        public ApplicationSettings Clone()
        {
            return (ApplicationSettings)MemberwiseClone();
        }

        public void Validate()
        {
            MaximumRecentFolders = Math.Max(1,
                Math.Min(100, MaximumRecentFolders));
            FolderSizeScanConcurrency = Math.Max(1,
                Math.Min(8, FolderSizeScanConcurrency));
            SmallSizeUpperLimitMb = Math.Max(1,
                SmallSizeUpperLimitMb);
            MediumSizeUpperLimitMb = Math.Max(
                SmallSizeUpperLimitMb + 1,
                MediumSizeUpperLimitMb);
            HomeFolder = (HomeFolder ?? string.Empty).Trim();
            ThemeName = (ThemeName ?? string.Empty).Trim();
            PaletteName = (PaletteName ?? string.Empty).Trim();
            LastFolder = (LastFolder ?? string.Empty).Trim();
            EverythingCliPath = (EverythingCliPath ?? string.Empty).Trim();
        }
    }
}
