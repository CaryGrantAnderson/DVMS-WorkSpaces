using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DVMSFileManager
{
    internal sealed class WiwaControl : XtraUserControl
    {
        private readonly OperationLogDatabase database;
        private readonly ComboBoxEdit datePeriod = new ComboBoxEdit();
        private readonly DateEdit specificDate = new DateEdit();
        private readonly ComboBoxEdit viewFilter = new ComboBoxEdit();
        private readonly SimpleButton refresh = new SimpleButton();
        private readonly SimpleButton close = new SimpleButton();
        private readonly SimpleButton print = new SimpleButton();
        private readonly SimpleButton printPreview = new SimpleButton();
        private readonly DropDownButton export = new DropDownButton();
        private readonly BarManager exportBarManager = new BarManager();
        private readonly PopupMenu exportMenu = new PopupMenu();
        private readonly LabelControl summary = new LabelControl();
        private readonly GridControl grid = new GridControl();
        private readonly GridView view;

        public event EventHandler CloseRequested;

        public WiwaControl(OperationLogDatabase database)
        {
            this.database = database;

            var top = new PanelControl { Dock = DockStyle.Top, Height = 48 };
            var dateLabel = new LabelControl { Text = "Date:" };
            dateLabel.SetBounds(8, 15, 38, 20);
            datePeriod.SetBounds(48, 10, 185, 24);
            datePeriod.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            datePeriod.Properties.Items.AddRange(new object[]
            {
                "Today - Last 6 Hours",
                "Today (All Day)",
                "Yesterday",
                "Day Before Yesterday",
                "Specific Day"
            });
            datePeriod.SelectedIndex = 0;

            specificDate.SetBounds(241, 10, 125, 24);
            specificDate.Properties.CalendarTimeEditing =
                DevExpress.Utils.DefaultBoolean.False;
            specificDate.DateTime = DateTime.Today;
            specificDate.Visible = false;

            var viewLabel = new LabelControl { Text = "View:" };
            viewLabel.SetBounds(382, 15, 38, 20);
            viewFilter.SetBounds(422, 10, 175, 24);
            viewFilter.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            viewFilter.Properties.Items.AddRange(new object[]
            {
                "All Actions",
                "Folders",
                "Documents Opened/Etc.",
                "Apps Directly Run",
                "Other Actions"
            });
            viewFilter.SelectedIndex = 0;

            refresh.Text = "Refresh";
            refresh.SetBounds(608, 8, 75, 28);
            print.Text = "Print";
            print.SetBounds(691, 8, 70, 28);
            printPreview.Text = "Print Preview";
            printPreview.SetBounds(769, 8, 105, 28);
            export.Text = "Export";
            export.SetBounds(882, 8, 90, 28);
            close.Text = "Close";
            close.SetBounds(980, 8, 75, 28);
            summary.SetBounds(1068, 15, 300, 20);
            top.Controls.AddRange(new Control[]
            {
                dateLabel, datePeriod, specificDate, viewLabel,
                viewFilter, refresh, print, printPreview, export,
                close, summary
            });

            ConfigureExportMenu();

            view = new GridView(grid);
            grid.MainView = view;
            grid.ViewCollection.Add(view);
            grid.Dock = DockStyle.Fill;
            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsSelection.MultiSelect = true;

            Controls.Add(grid);
            Controls.Add(top);

            datePeriod.SelectedIndexChanged += (sender, e) =>
            {
                specificDate.Visible = datePeriod.SelectedIndex == 4;
                RefreshTimeline();
            };
            specificDate.EditValueChanged += (sender, e) =>
            {
                if (datePeriod.SelectedIndex == 4)
                    RefreshTimeline();
            };
            viewFilter.SelectedIndexChanged += (sender, e) =>
                RefreshTimeline();
            refresh.Click += (sender, e) => RefreshTimeline();
            print.Click += (sender, e) => grid.Print();
            printPreview.Click += (sender, e) => grid.ShowPrintPreview();
            close.Click += (sender, e) =>
                CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ConfigureExportMenu()
        {
            exportBarManager.Form = this;
            exportMenu.Manager = exportBarManager;
            AddExportCommand("CSV", "csv", "CSV files (*.csv)|*.csv",
                path => grid.ExportToCsv(path));
            AddExportCommand("Excel", "xlsx", "Excel files (*.xlsx)|*.xlsx",
                path => grid.ExportToXlsx(path));
            AddExportCommand("PDF", "pdf", "PDF files (*.pdf)|*.pdf",
                path => grid.ExportToPdf(path));
            AddExportCommand("DOCX", "docx", "Word files (*.docx)|*.docx",
                path => grid.ExportToDocx(path));
            AddExportCommand("RTF", "rtf", "Rich Text files (*.rtf)|*.rtf",
                path => grid.ExportToRtf(path));
            AddExportCommand("TXT", "txt", "Text files (*.txt)|*.txt",
                path => grid.ExportToText(path));
            AddExportCommand("HTML", "html", "HTML files (*.html)|*.html",
                path => grid.ExportToHtml(path));
            AddExportCommand("MHT", "mht", "MHT files (*.mht)|*.mht",
                path => grid.ExportToMht(path));
            export.DropDownControl = exportMenu;
        }

        private void AddExportCommand(
            string caption,
            string extension,
            string filter,
            Action<string> exportAction)
        {
            var item = new BarButtonItem(exportBarManager, caption);
            item.ItemClick += (sender, e) =>
                ExportTimeline(extension, filter, exportAction);
            exportMenu.AddItem(item);
        }

        private void ExportTimeline(
            string extension,
            string filter,
            Action<string> exportAction)
        {
            using (var dialog = new SaveFileDialog
            {
                Title = "Export WIWA Timeline",
                Filter = filter,
                DefaultExt = extension,
                AddExtension = true,
                FileName = "WIWA-Timeline-" +
                    DateTime.Now.ToString("yyyy-MM-dd") + "." + extension
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    exportAction(dialog.FileName);
                }
                catch (Exception exception)
                {
                    XtraMessageBox.Show(
                        this, exception.Message, "Export WIWA Timeline",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void RefreshTimeline()
        {
            try
            {
                DateTime start;
                DateTime end;
                GetDateRange(out start, out end);
                DataTable raw = database.LoadTimeline(start, end);
                DataTable timeline = CreateTimelineTable();
                string selectedCategory = Convert.ToString(
                    viewFilter.EditValue) ?? "All Actions";

                foreach (DataRow row in raw.Rows)
                {
                    string operation = Convert.ToString(row["Operation"]);
                    string source = Convert.ToString(row["SourcePath"]);
                    string category = GetCategory(operation, source);
                    if (selectedCategory != "All Actions" &&
                        !string.Equals(category, selectedCategory,
                            StringComparison.Ordinal))
                        continue;

                    DateTime occurredUtc = DateTime.Parse(
                        Convert.ToString(row["OccurredUtc"]),
                        null,
                        System.Globalization.DateTimeStyles.RoundtripKind);
                    timeline.Rows.Add(
                        occurredUtc.ToLocalTime(),
                        category,
                        operation,
                        GetItemName(source),
                        source,
                        Convert.ToString(row["TargetPath"]),
                        Convert.ToString(row["Details"]),
                        Convert.ToInt32(row["WasSuccessful"]) != 0
                            ? "Yes" : "No",
                        Convert.ToString(row["ErrorMessage"]));
                }

                grid.DataSource = timeline;
                ConfigureColumns();
                summary.Text = timeline.Rows.Count + " operations · " +
                    start.ToString("g") + " to " + end.ToString("g");
            }
            catch (Exception exception)
            {
                summary.Text = "Unable to load timeline";
                XtraMessageBox.Show(
                    this, exception.Message, "WIWA",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GetDateRange(out DateTime start, out DateTime end)
        {
            DateTime today = DateTime.Today;
            switch (datePeriod.SelectedIndex)
            {
                case 1:
                    start = today; end = today.AddDays(1); break;
                case 2:
                    start = today.AddDays(-1); end = today; break;
                case 3:
                    start = today.AddDays(-2); end = today.AddDays(-1); break;
                case 4:
                    start = specificDate.DateTime.Date;
                    end = start.AddDays(1);
                    break;
                default:
                    end = DateTime.Now;
                    start = end.AddHours(-6);
                    break;
            }
        }

        private static string GetCategory(string operation, string source)
        {
            if (string.Equals(operation, "Navigate Folder",
                StringComparison.OrdinalIgnoreCase))
                return "Folders";
            if (string.Equals(operation, "Launch Application",
                StringComparison.OrdinalIgnoreCase))
                return "Apps Directly Run";
            if (operation != null &&
                operation.StartsWith("File Command:",
                    StringComparison.OrdinalIgnoreCase))
                return "Documents Opened/Etc.";
            return "Other Actions";
        }

        private static string GetItemName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            string trimmed = path.TrimEnd('\\', '/');
            string name = Path.GetFileName(trimmed);
            return string.IsNullOrWhiteSpace(name) ? path : name;
        }

        private static DataTable CreateTimelineTable()
        {
            var table = new DataTable();
            table.Columns.Add("Date and Time", typeof(DateTime));
            table.Columns.Add("View", typeof(string));
            table.Columns.Add("Action", typeof(string));
            table.Columns.Add("Item", typeof(string));
            table.Columns.Add("Location", typeof(string));
            table.Columns.Add("Target", typeof(string));
            table.Columns.Add("Details", typeof(string));
            table.Columns.Add("Successful", typeof(string));
            table.Columns.Add("Error", typeof(string));
            return table;
        }

        private void ConfigureColumns()
        {
            view.BestFitColumns();
            var dateColumn = view.Columns["Date and Time"];
            if (dateColumn != null)
            {
                dateColumn.DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.DateTime;
                dateColumn.DisplayFormat.FormatString = "g";
                dateColumn.Width = Math.Max(135, dateColumn.Width);
            }
            var locationColumn = view.Columns["Location"];
            if (locationColumn != null)
                locationColumn.Width = Math.Min(350,
                    Math.Max(180, locationColumn.Width));
        }
    }
}
