using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace DVMS.ButtonBar;

internal class ButtonBarForm : XtraForm
{
    private const int ScMove = 0xF010;
    private readonly ButtonBarDatabase _database;
    private readonly FlowLayoutPanel _flow;
    private readonly BarManager _barManager = new();
    private readonly PopupMenu _buttonMenu = new();
    private Bar _mainMenu = null!;
    private ButtonBarSettings _settings;
    private ButtonDefinition? _selectedButton;
    private bool _restoringBounds;
    private readonly bool _hexMode;

    private BarCheckItem _showLabelsItem = null!;
    private BarCheckItem _alwaysOnTopItem = null!;
    private BarCheckItem _showMenuItem = null!;
    private BarCheckItem _lockEditingItem = null!;
    private BarCheckItem _lockResizeItem = null!;
    private BarCheckItem _lockMoveItem = null!;

    public ButtonBarForm(ButtonBarDatabase database, bool hexMode = false)
    {
        _database = database;
        _hexMode = hexMode;
        _flow = hexMode ? new HexFlowPanel() : new FlowLayoutPanel();
        _settings = database.LoadSettings();

        Text = hexMode ? "DVMS Hex Button Bar" : "DVMS Button Bar";
        MinimumSize = new Size(145, 150);
        StartPosition = FormStartPosition.Manual;
        AllowDrop = true;

        ConfigureFlowPanel();
        ConfigureMenus();
        RestoreSettings();
        ReloadButtons();

        DragEnter += OnDragEnter;
        DragDrop += OnDragDrop;
        Move += (_, _) => SaveBounds();
        ResizeEnd += (_, _) => SaveBounds();
        FormClosing += (_, _) => SaveBounds();
    }

    protected override void WndProc(ref Message message)
    {
        if (_settings.LockWindowMovement && message.Msg == 0x0112 && ((int)message.WParam & 0xFFF0) == ScMove)
        {
            return;
        }
        base.WndProc(ref message);
    }

    private void ConfigureFlowPanel()
    {
        _flow.Dock = DockStyle.Fill;
        _flow.AutoScroll = true;
        _flow.WrapContents = true;
        _flow.FlowDirection = FlowDirection.LeftToRight;
        _flow.Padding = new Padding(8);
        _flow.AllowDrop = true;
        _flow.DragEnter += OnDragEnter;
        _flow.DragDrop += OnDragDrop;
        Controls.Add(_flow);
    }

    private void ConfigureMenus()
    {
        _barManager.Form = this;
        _mainMenu = new Bar(_barManager, "Main Menu") { DockStyle = BarDockStyle.Top };
        _barManager.MainMenu = _mainMenu;

        var addItem = Item("Add", (_, _) => AddProgram(), "AddProgram");

        var settingsMenu = new BarSubItem(_barManager, "Settings");
        settingsMenu.AddItem(Item("View Launched Program Log...", (_, _) => ShowLaunchHistory()));
        settingsMenu.AddItem(Item("View Opened File Log...", (_, _) => ShowFileActivity()));
        settingsMenu.AddItem(Item("Sound Folder...", (_, _) => ChooseSoundDirectory()));
        settingsMenu.AddItem(Item("Open Data Folder", (_, _) => OpenDataFolder()));
        settingsMenu.AddItem(Item("Exit", (_, _) => Close()));

        var viewMenu = new BarSubItem(_barManager, "View");
        _showLabelsItem = CheckItem("Show Button Names", _settings.ShowButtonLabels, (_, _) =>
        {
            Save("ShowButtonLabels", _showLabelsItem.Checked);
            ReloadButtons();
        });
        _alwaysOnTopItem = CheckItem("Always on Top", _settings.AlwaysOnTop, (_, _) =>
        {
            Save("AlwaysOnTop", _alwaysOnTopItem.Checked);
            TopMost = _alwaysOnTopItem.Checked;
        });
        viewMenu.AddItem(_showLabelsItem);
        viewMenu.AddItem(_alwaysOnTopItem);
        _showMenuItem = CheckItem("Show Menu Bar", _settings.ShowMenuBar, (_, _) =>
            SetMenuVisibility(_showMenuItem.Checked));
        viewMenu.AddItem(_showMenuItem);
        var sizeMenu = new BarSubItem(_barManager, "Button Size");
        foreach (var size in Enum.GetValues<ButtonSize>())
        {
            var sizeItem = Item(size.ToString(), (_, _) =>
            {
                Save("ButtonSize", size);
                ReloadButtons();
            });
            sizeMenu.AddItem(sizeItem);
        }
        viewMenu.AddItem(sizeMenu);

        var lockMenu = new BarSubItem(_barManager, "Lock");
        _lockEditingItem = CheckItem("Lock Button Changes", _settings.LockButtonEditing, (_, _) =>
        {
            Save("LockButtonEditing", _lockEditingItem.Checked);
            UpdateEditingCommands();
        });
        _lockResizeItem = CheckItem("Lock Window Size", _settings.LockWindowResize, (_, _) =>
        {
            Save("LockWindowResize", _lockResizeItem.Checked);
            ApplyResizeLock();
        });
        _lockMoveItem = CheckItem("Lock Window Position", _settings.LockWindowMovement, (_, _) =>
            Save("LockWindowMovement", _lockMoveItem.Checked));
        lockMenu.AddItems(new BarItem[] { _lockEditingItem, _lockResizeItem, _lockMoveItem });

        var helpMenu = new BarSubItem(_barManager, "Help");
        helpMenu.AddItem(Item("About Button Bar", (_, _) => XtraMessageBox.Show(
            this,
            $".NET 9 • DevExpress 25.1 • SQLite\n\n{(_hexMode ? "Hexagonal honeycomb layout. " : "")}Drag programs and shortcuts onto the bar to add them.",
            _hexMode ? "DVMS Hex Button Bar" : "DVMS Button Bar")));

        _mainMenu.AddItems(new BarItem[] { addItem, settingsMenu, viewMenu, lockMenu, helpMenu });
        ConfigureButtonPopup();
    }

    private void ConfigureButtonPopup()
    {
        _buttonMenu.Manager = _barManager;
        _buttonMenu.AddItem(Item("Launch", (_, _) => LaunchSelected()));
        _buttonMenu.AddItem(Item("Edit...", (_, _) => EditSelected(), "EditButton"));
        if (_hexMode)
            _buttonMenu.AddItem(Item("Appearance...", (_, _) => EditHexAppearance(), "HexAppearance"));
        _buttonMenu.AddItem(Item("Clone", (_, _) => CloneSelected(), "CloneButton"));
        _buttonMenu.AddItem(Item("Move Left", (_, _) => MoveSelected(-1), "MoveLeft"));
        _buttonMenu.AddItem(Item("Move Right", (_, _) => MoveSelected(1), "MoveRight"));
        _buttonMenu.AddItem(Item("Remove", (_, _) => RemoveSelected(), "RemoveButton"));
        _buttonMenu.AddItem(Item("Hide / View Menu", (_, _) => SetMenuVisibility(!_mainMenu.Visible), "ToggleMenu"));
    }

    private BarButtonItem Item(string caption, ItemClickEventHandler handler, string? name = null)
    {
        var item = new BarButtonItem(_barManager, caption) { Name = name ?? $"Item{_barManager.Items.Count}" };
        item.ItemClick += handler;
        return item;
    }

    private BarCheckItem CheckItem(string caption, bool value, ItemClickEventHandler handler)
    {
        var item = new BarCheckItem(_barManager) { Caption = caption, Checked = value };
        item.ItemClick += handler;
        return item;
    }

    private void ReloadButtons()
    {
        _settings = _database.LoadSettings();
        UpdateMinimumWidth();
        _flow.SuspendLayout();
        try
        {
            while (_flow.Controls.Count > 0)
            {
                _flow.Controls[0].Dispose();
            }

            foreach (var definition in _database.LoadButtons())
            {
                _flow.Controls.Add(CreateLaunchButton(definition));
            }
        }
        finally
        {
            _flow.ResumeLayout();
        }
        UpdateEditingCommands();
    }

    private Control CreateLaunchButton(ButtonDefinition definition)
    {
        var (size, iconSize) = _settings.ButtonSize switch
        {
            ButtonSize.Small => (new Size(88, 62), new Size(32, 32)),
            ButtonSize.Large => (new Size(150, 112), new Size(64, 64)),
            _ => (new Size(118, 88), new Size(48, 48))
        };
        if (!_settings.ShowButtonLabels)
        {
            size = new Size(size.Height, size.Height);
        }

        Control button;
        if (_hexMode)
        {
            // Point-up regular hexagons are slightly narrower than they are tall.
            size = new Size((int)Math.Round(size.Height * .90), size.Height);
            var appearance = _database.LoadHexAppearance(definition.Id);
            button = new HexLaunchButton
            {
                Text = _settings.ShowButtonLabels && appearance.ShowLabel ? definition.DisplayName : string.Empty,
                Size = size,
                Tag = definition,
                ToolTipText = definition.DisplayName,
                Icon = appearance.ShowIcon ? IconLoader.Load(definition, iconSize) : null,
                Appearance = appearance
            };
        }
        else
        {
            button = new SimpleButton
            {
                Text = _settings.ShowButtonLabels ? definition.DisplayName : string.Empty,
                Size = size,
                Tag = definition,
                ToolTip = definition.DisplayName,
                ImageOptions = { Image = IconLoader.Load(definition, iconSize), ImageToTextAlignment = ImageAlignToText.TopCenter }
            };
        }
        button.Click += (_, _) => Launch(definition);
        button.MouseUp += (_, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                _selectedButton = definition;
                UpdateEditingCommands();
                if (_barManager.Items["ToggleMenu"] is BarItem toggleMenu)
                    toggleMenu.Caption = _mainMenu.Visible ? "Hide Menu" : "View Menu";
                _buttonMenu.ShowPopup(button.PointToScreen(e.Location));
            }
        };
        return button;
    }

    private void AddProgram(string? initialPath = null)
    {
        if (_settings.LockButtonEditing)
        {
            ShowLockedMessage();
            return;
        }

        ButtonDefinition? initial = null;
        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            initial = new ButtonDefinition(0, Path.GetFileNameWithoutExtension(initialPath), initialPath,
                "", "", "", "", false, 0);
        }
        using var dialog = new ButtonEditorForm(_settings.SoundDirectory, initial);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _database.AddButton(dialog.GetDefinition());
            ReloadButtons();
        }
    }

    private void EditSelected()
    {
        if (_selectedButton is null || _settings.LockButtonEditing) return;
        using var dialog = new ButtonEditorForm(_settings.SoundDirectory, _selectedButton);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _database.UpdateButton(dialog.GetDefinition(_selectedButton.Id, _selectedButton.SortOrder));
            ReloadButtons();
        }
    }

    private void EditHexAppearance()
    {
        if (!_hexMode || _selectedButton is null || _settings.LockButtonEditing) return;
        using var dialog = new HexAppearanceForm(_selectedButton.DisplayName,
            _database.LoadHexAppearance(_selectedButton.Id));
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _database.SaveHexAppearance(dialog.SelectedAppearance);
            ReloadButtons();
        }
    }

    private void RemoveSelected()
    {
        if (_selectedButton is null || _settings.LockButtonEditing) return;
        if (XtraMessageBox.Show(this, $"Remove '{_selectedButton.DisplayName}' from Button Bar?",
                "Remove button", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _database.DeleteButton(_selectedButton.Id);
            _selectedButton = null;
            ReloadButtons();
        }
    }

    private void CloneSelected()
    {
        if (_selectedButton is null || _settings.LockButtonEditing) return;
        var clone = _selectedButton with { Id = 0, DisplayName = $"{_selectedButton.DisplayName} Copy", SortOrder = 0 };
        using var dialog = new ButtonEditorForm(_settings.SoundDirectory, clone);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _database.AddButton(dialog.GetDefinition());
            ReloadButtons();
        }
    }

    private void MoveSelected(int direction)
    {
        if (_selectedButton is null || _settings.LockButtonEditing) return;
        _database.MoveButton(_selectedButton.Id, direction);
        ReloadButtons();
    }

    private void LaunchSelected()
    {
        if (_selectedButton is not null) Launch(_selectedButton);
    }

    private void Launch(ButtonDefinition definition)
    {
        var fileMonitoringStatus = "Not requested";
        var fileMonitoringReady = !definition.MonitorFileActivity
                                  || FileActivityMonitor.Instance.Prepare(out fileMonitoringStatus);
        var result = LauncherService.Launch(definition, _settings.SoundDirectory);
        if (result.Started)
        {
            var historyId = _database.LogLaunch(definition, result.Process);
            if (result.Process is not null)
            {
                ProcessTrackingService.Track(
                    result.Process,
                    definition,
                    historyId,
                    _database,
                    Screen.FromControl(this));

                if (definition.MonitorFileActivity)
                {
                    if (fileMonitoringReady)
                    {
                        FileActivityMonitor.Instance.Track(result.Process.Id, historyId, _database);
                        _database.UpdateFileMonitoringStatus(historyId, "Monitoring");
                    }
                    else
                    {
                        _database.UpdateFileMonitoringStatus(historyId, fileMonitoringStatus!);
                    }
                }
            }
            else if (definition.MonitorFileActivity)
            {
                _database.UpdateFileMonitoringStatus(
                    historyId, "Unavailable - launch did not return a process handle");
            }
        }
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = !_settings.LockButtonEditing && e.Data?.GetDataPresent(DataFormats.FileDrop) == true
            ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (_settings.LockButtonEditing || e.Data?.GetData(DataFormats.FileDrop) is not string[] paths) return;
        foreach (var path in paths.Where(File.Exists))
        {
            AddProgram(path);
        }
    }

    private void RestoreSettings()
    {
        _restoringBounds = true;
        try
        {
            var fallback = new Rectangle(100, 100, 540, 230);
            var bounds = new Rectangle(
                _settings.WindowX ?? fallback.X, _settings.WindowY ?? fallback.Y,
                _settings.WindowWidth ?? fallback.Width, _settings.WindowHeight ?? fallback.Height);
            Bounds = EnsureVisible(bounds);
            TopMost = _settings.AlwaysOnTop;
            ApplyResizeLock();
            _mainMenu.Visible = _settings.ShowMenuBar;
        }
        finally
        {
            _restoringBounds = false;
        }
    }

    private void ApplyResizeLock()
    {
        var current = Bounds;
        FormBorderStyle = _settings.LockWindowResize ? FormBorderStyle.FixedToolWindow : FormBorderStyle.SizableToolWindow;
        Bounds = current;
        MaximizeBox = !_settings.LockWindowResize;
    }

    private void UpdateEditingCommands()
    {
        var enabled = !_settings.LockButtonEditing;
        foreach (var name in new[] { "EditButton", "HexAppearance", "CloneButton", "MoveLeft", "MoveRight", "RemoveButton" })
        {
            if (_barManager.Items[name] is BarItem item) item.Enabled = enabled;
        }
    }

    private void Save(string key, object value)
    {
        _database.SaveSetting(key, value);
        _settings = _database.LoadSettings();
    }

    private void SaveBounds()
    {
        if (!_restoringBounds && WindowState == FormWindowState.Normal)
        {
            _database.SaveWindowBounds(Bounds);
        }
    }

    private void OpenDataFolder()
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = Path.GetDirectoryName(_database.DatabasePath)!,
            UseShellExecute = true
        });
    }

    private void ShowLaunchHistory()
    {
        using var dialog = new LaunchHistoryForm(_database);
        dialog.ShowDialog(this);
    }

    private void ShowFileActivity()
    {
        using var dialog = new FileActivityForm(_database);
        dialog.ShowDialog(this);
    }

    private void ChooseSoundDirectory()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Choose the default folder for Button Bar sound files",
            SelectedPath = Directory.Exists(_settings.SoundDirectory) ? _settings.SoundDirectory : string.Empty
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            Save("SoundDirectory", dialog.SelectedPath);
        }
    }

    private void SetMenuVisibility(bool visible)
    {
        if (_mainMenu.Visible == visible) return;

        var contentHeight = _flow.Height;
        _mainMenu.Visible = visible;
        PerformLayout();
        var heightAdjustment = contentHeight - _flow.Height;
        Height = Math.Max(MinimumSize.Height, Height + heightAdjustment);
        _showMenuItem.Checked = visible;
        Save("ShowMenuBar", visible);
        SaveBounds();
    }

    private void ShowLockedMessage() => XtraMessageBox.Show(
        this, "Button changes are locked. Turn off Lock > Lock Button Changes first.",
        _hexMode ? "Hex Button Bar is locked" : "Button Bar is locked");

    private Rectangle EnsureVisible(Rectangle bounds)
    {
        var screen = Screen.FromRectangle(bounds).WorkingArea;
        var width = Math.Clamp(bounds.Width, MinimumSize.Width, screen.Width);
        var height = Math.Clamp(bounds.Height, 150, screen.Height);
        return new Rectangle(
            Math.Clamp(bounds.X, screen.Left, screen.Right - width),
            Math.Clamp(bounds.Y, screen.Top, screen.Bottom - height), width, height);
    }

    private void UpdateMinimumWidth()
    {
        var buttonWidth = _settings.ButtonSize switch
        {
            ButtonSize.Small => _settings.ShowButtonLabels ? 88 : 62,
            ButtonSize.Large => _settings.ShowButtonLabels ? 150 : 112,
            _ => _settings.ShowButtonLabels ? 118 : 88
        };
        if (_hexMode) buttonWidth = (int)Math.Round(buttonWidth * .90);
        MinimumSize = new Size(buttonWidth + 48, 150);
    }
}
