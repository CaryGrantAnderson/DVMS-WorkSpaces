using DevExpress.XtraBars;

namespace DVMS.StartTree;

internal sealed class LauncherIconForm : Form
{
    private const int DragThreshold = 5;
    private readonly StartTreeDatabase _database;
    private readonly LauncherIconControl _icon = new() { Dock = DockStyle.Fill };
    private readonly PopupMenu _contextMenu = new();
    private readonly BarManager _barManager = new();
    private Point _mouseDownScreen;
    private Point _formDownLocation;
    private bool _rightButtonDown;
    private bool _dragged;
    private ProgramTreeForm? _programTree;

    public LauncherIconForm(StartTreeDatabase database)
    {
        _database = database;
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.Magenta;
        ClientSize = new Size(72, 72);
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        TransparencyKey = Color.Magenta;
        Text = "DVMS Start Tree";

        Controls.Add(_icon);
        ConfigureContextMenu();

        _icon.MouseDown += IconMouseDown;
        _icon.MouseMove += IconMouseMove;
        _icon.MouseUp += IconMouseUp;
        _icon.MouseClick += IconMouseClick;

        var settings = _database.LoadSettings();
        Location = RestoreLocation(settings);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            const int toolWindow = 0x80;
            var parameters = base.CreateParams;
            parameters.ExStyle |= toolWindow;
            return parameters;
        }
    }

    private void ConfigureContextMenu()
    {
        _barManager.Form = this;
        _contextMenu.Manager = _barManager;
        var settingsItem = new BarButtonItem(_barManager, "Settings");
        settingsItem.ItemClick += (_, _) => ShowSettings();
        _contextMenu.AddItem(settingsItem);
    }

    private void IconMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        _rightButtonDown = true;
        _dragged = false;
        _mouseDownScreen = _icon.PointToScreen(e.Location);
        _formDownLocation = Location;
        _icon.Capture = true;
    }

    private void IconMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_rightButtonDown || (Control.MouseButtons & MouseButtons.Right) == 0)
        {
            return;
        }

        var current = _icon.PointToScreen(e.Location);
        var delta = new Size(current.X - _mouseDownScreen.X, current.Y - _mouseDownScreen.Y);
        if (!_dragged && Math.Abs(delta.Width) < DragThreshold && Math.Abs(delta.Height) < DragThreshold)
        {
            return;
        }

        _dragged = true;
        Location = ClampToVirtualScreen(_formDownLocation + delta);
    }

    private void IconMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right || !_rightButtonDown)
        {
            return;
        }

        _rightButtonDown = false;
        _icon.Capture = false;

        if (_dragged)
        {
            _database.SaveIconLocation(Location);
            return;
        }

        _contextMenu.ShowPopup(_icon.PointToScreen(e.Location));
    }

    private void IconMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ToggleProgramTree();
        }
    }

    private void ToggleProgramTree()
    {
        if (_programTree is { Visible: true })
        {
            _programTree.Hide();
            return;
        }

        _programTree ??= new ProgramTreeForm(_database);
        _programTree.Reload();
        _programTree.ShowNear(this);
    }

    private void ShowSettings()
    {
        using var dialog = new SettingsForm(_database.LoadSettings());
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _database.SaveKeepMenuOpenAfterLaunch(dialog.KeepMenuOpenAfterLaunch);
        }
    }

    private Point RestoreLocation(AppSettings settings)
    {
        if (settings.IconX is int x && settings.IconY is int y)
        {
            return ClampToVirtualScreen(new Point(x, y));
        }

        var workArea = Screen.PrimaryScreen?.WorkingArea ?? SystemInformation.VirtualScreen;
        return new Point(workArea.Right - Width - 24, workArea.Top + 24);
    }

    private Point ClampToVirtualScreen(Point location)
    {
        var screen = Screen.FromPoint(location).WorkingArea;
        return new Point(
            Math.Clamp(location.X, screen.Left, Math.Max(screen.Left, screen.Right - Width)),
            Math.Clamp(location.Y, screen.Top, Math.Max(screen.Top, screen.Bottom - Height)));
    }
}
