using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace DVMS.ButtonBar;

internal sealed class FileActivityForm : XtraForm
{
    private readonly ButtonBarDatabase _database;
    private readonly GridControl _grid = new();
    private readonly GridView _view = new();

    public FileActivityForm(ButtonBarDatabase database)
    {
        _database = database;
        Text = "Opened File Log";
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimumSize = new Size(760, 380);
        ClientSize = new Size(1120, 600);

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
        _grid.DataSource = _database.LoadFileActivity();
        _view.PopulateColumns();

        Configure(nameof(FileActivityEntry.OpenedAtLocal), "Opened", 170, "g");
        Configure(nameof(FileActivityEntry.ClosedAtLocal), "Closed", 170, "g");
        Configure(nameof(FileActivityEntry.ProcessName), "Process", 140);
        Configure(nameof(FileActivityEntry.ProcessId), "Process ID", 85);
        Configure(nameof(FileActivityEntry.FilePath), "File", 430);
        Configure(nameof(FileActivityEntry.AccessType), "Access", 100);
        Configure(nameof(FileActivityEntry.DurationSeconds), "Duration (sec)", 110, "n1");
        Configure(nameof(FileActivityEntry.TrackingStatus), "Status", 90);

        if (_view.Columns[nameof(FileActivityEntry.Id)] is { } id) id.Visible = false;
        if (_view.Columns[nameof(FileActivityEntry.LaunchHistoryId)] is { } launchId) launchId.Visible = false;
    }

    private void Configure(string field, string caption, int width, string? format = null)
    {
        var column = _view.Columns[field];
        if (column is null) return;
        column.Caption = caption;
        column.Width = width;
        if (format is null) return;
        column.DisplayFormat.FormatType = field == nameof(FileActivityEntry.DurationSeconds)
            ? DevExpress.Utils.FormatType.Numeric
            : DevExpress.Utils.FormatType.DateTime;
        column.DisplayFormat.FormatString = format;
    }
}
