using DevExpress.XtraEditors;
using System;
using System.IO;
using System.Windows.Forms;

namespace DVMSFileManager
{
    internal partial class SettingsForm : XtraForm
    {
        private readonly string defaultHomeFolder;
        private readonly string defaultThemeName;
        private readonly string defaultPaletteName;

        public SettingsForm(
            ApplicationSettings settings,
            string settingsDatabasePath,
            string defaultHomeFolder,
            string defaultThemeName,
            string defaultPaletteName)
        {
            InitializeComponent();
            this.defaultHomeFolder = defaultHomeFolder;
            this.defaultThemeName = defaultThemeName;
            this.defaultPaletteName = defaultPaletteName;
            Settings = settings.Clone();
            propertyGridControl_Settings.SelectedObject = Settings;
            labelControl_DatabasePath.Text = settingsDatabasePath;
        }

        public ApplicationSettings Settings { get; private set; }

        public event EventHandler SettingsApplied;

        private void simpleButton_BrowseHome_Click(object sender, EventArgs e)
        {
            using (var dialog = new XtraFolderBrowserDialog())
            {
                dialog.SelectedPath = Directory.Exists(Settings.HomeFolder)
                    ? Settings.HomeFolder
                    : defaultHomeFolder;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    Settings.HomeFolder = dialog.SelectedPath;
                    propertyGridControl_Settings.Refresh();
                }
            }
        }

        private void simpleButton_Defaults_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show(
                    this,
                    "Restore all File Manager settings to their defaults?",
                    "Restore Defaults",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            Settings = ApplicationSettings.CreateDefaults(defaultHomeFolder);
            propertyGridControl_Settings.SelectedObject = Settings;
        }

        private void simpleButton_DefaultTheme_Click(
            object sender,
            EventArgs e)
        {
            Settings.ThemeName = defaultThemeName;
            Settings.PaletteName = defaultPaletteName;
            propertyGridControl_Settings.Refresh();
        }

        private bool ApplySettings()
        {
            propertyGridControl_Settings.CloseEditor();
            Settings.Validate();

            if (string.IsNullOrWhiteSpace(Settings.HomeFolder) ||
                !Directory.Exists(Settings.HomeFolder))
            {
                XtraMessageBox.Show(
                    this,
                    "Please select an existing Home folder.",
                    "Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            SettingsApplied?.Invoke(this, EventArgs.Empty);
            return true;
        }

        private void simpleButton_Apply_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void simpleButton_OK_Click(object sender, EventArgs e)
        {
            if (!ApplySettings())
                return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
