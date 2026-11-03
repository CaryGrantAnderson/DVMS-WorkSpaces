using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace DVMS.ButtonBar;

internal sealed class LaunchHistoryForm : XtraForm
{
    private readonly ButtonBarDatabase _database;
    private readonly GridControl _grid = new();
    private readonly GridView _view = new();

    public LaunchHistoryForm(ButtonBarDatabase database)
    {
        _database = database;
        Text = "Launched Program Log";
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimumSize = new Size(700, 360);
        ClientSize = new Size(980, 560);

        _grid.Dock = DockStyle.Fill;
        _grid.MainView = _view;
        _grid.ViewCollection.Add(_view);
        _view.OptionsBehavior.Editable = false;
        _view.OptionsBehavior.ReadOnly = true;
        _view.OptionsSelection.EnableAppearanceFocusedCell = false;
        _view.OptionsView.ShowAutoFilterRow = true;
        _view.OptionsView.ShowGroupPanel = false;
        _view.OptionsView.ColumnAutoWidth = false;

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(8)
        };
        var close = new SimpleButton { Text = "Close", DialogResult = DialogResult.OK, Size = new Size(90, 30) };
        var refresh = new SimpleButton { Text = "Refresh", Size = new Size(90, 30) };
        refresh.Click += (_, _) => Reload();
        actions.Controls.Add(close);
        actions.Controls.Add(refresh);

        Controls.Add(_grid);
        Controls.Add(actions);
        AcceptButton = close;
        CancelButton = close;
        Shown += (_, _) => Reload();
    }

    private void Reload()
    {
        _grid.DataSource = _database.LoadLaunchHistory();
        _view.PopulateColumns();

        ConfigureColumn(nameof(LaunchHistoryEntry.LaunchedAtLocal), "Launched (Local)", 170, "g");
        ConfigureColumn(nameof(LaunchHistoryEntry.DisplayName), "Button", 150);
        ConfigureColumn(nameof(LaunchHistoryEntry.LaunchTarget), "Program / Target", 300);
        ConfigureColumn(nameof(LaunchHistoryEntry.Arguments), "Arguments", 220);
        ConfigureColumn(nameof(LaunchHistoryEntry.TrackingStatus), "Status", 145);
        ConfigureColumn(nameof(LaunchHistoryEntry.ProcessId), "Process ID", 90);
        ConfigureColumn(nameof(LaunchHistoryEntry.EndedAtLocal), "Ended (Local)", 170, "g");
        ConfigureColumn(nameof(LaunchHistoryEntry.DurationSeconds), "Duration (sec)", 110, "n1");
        ConfigureColumn(nameof(LaunchHistoryEntry.ExitCode), "Exit Code", 80);
        ConfigureColumn(nameof(LaunchHistoryEntry.FileMonitoringStatus), "File Monitoring", 240);
        ConfigureColumn(nameof(LaunchHistoryEntry.LaunchedAtUtc), "Launched (UTC)", 170, "u");

        if (_view.Columns[nameof(LaunchHistoryEntry.Id)] is { } id) id.Visible = false;
        if (_view.Columns[nameof(LaunchHistoryEntry.ButtonId)] is { } buttonId) buttonId.Visible = false;
        if (_view.Columns[nameof(LaunchHistoryEntry.EndedAtUtc)] is { } endedUtc) endedUtc.Visible = false;
    }

    private void ConfigureColumn(string fieldName, string caption, int width, string? format = null)
    {
        var column = _view.Columns[fieldName];
        if (column is null) return;
        column.Caption = caption;
        column.Width = width;
        if (format is not null)
        {
            column.DisplayFormat.FormatType = fieldName == nameof(LaunchHistoryEntry.DurationSeconds)
                ? DevExpress.Utils.FormatType.Numeric
                : DevExpress.Utils.FormatType.DateTime;
            column.DisplayFormat.FormatString = format;
        }
    }
}
