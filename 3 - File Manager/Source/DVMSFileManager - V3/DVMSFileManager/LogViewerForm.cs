using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVMSFileManager
{
    internal sealed class LogViewerForm : XtraForm
    {
        private readonly OperationLogDatabase database;
        private readonly GridControl grid = new GridControl();
        private readonly GridView view;
        private readonly LabelControl status = new LabelControl();

        public LogViewerForm(OperationLogDatabase database)
        {
            this.database = database;
            Text = "DVMS File Manager — Operation Log";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1250, 720);
            MinimumSize = new Size(800, 450);

            var commands = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 45
            };
            var refresh = new SimpleButton
            {
                Text = "Refresh",
                Location = new Point(8, 8),
                Size = new Size(90, 28)
            };
            var openFolder = new SimpleButton
            {
                Text = "Open Log Folder",
                Location = new Point(106, 8),
                Size = new Size(130, 28)
            };
            status.Location = new Point(250, 14);
            commands.Controls.AddRange(
                new Control[] { refresh, openFolder, status });

            view = new GridView(grid);
            grid.MainView = view;
            grid.ViewCollection.Add(view);
            grid.Dock = DockStyle.Fill;
            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsView.ShowGroupPanel = true;
            view.OptionsSelection.MultiSelect = true;

            Controls.Add(grid);
            Controls.Add(commands);
            refresh.Click += (sender, e) => RefreshLog();
            openFolder.Click += (sender, e) => OpenLogFolder();
            Shown += (sender, e) => RefreshLog();
        }

        public void RefreshLog()
        {
            try
            {
                var data = database.LoadRecent();
                grid.DataSource = data;
                view.BestFitColumns();
                status.Text = data.Rows.Count + " most recent operations";
            }
            catch (Exception exception)
            {
                status.Text = "Unable to load log";
                XtraMessageBox.Show(
                    this, exception.Message, "Operation Log",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OpenLogFolder()
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = System.IO.Path.GetDirectoryName(
                        database.DatabasePath),
                    UseShellExecute = true
                });
        }
    }
}
