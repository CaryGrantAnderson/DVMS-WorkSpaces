using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace DVMS.ButtonBar;

internal sealed class ButtonEditorForm : XtraForm
{
    private readonly TextEdit _name = new();
    private readonly ButtonEdit _target = new();
    private readonly TextEdit _arguments = new();
    private readonly ButtonEdit _workingDirectory = new();
    private readonly ButtonEdit _iconPath = new();
    private readonly ButtonEdit _soundFile = new();
    private readonly CheckEdit _monitorFiles = new()
    {
        Text = "Monitor files opened and closed by this program and its child processes"
    };

    public ButtonEditorForm(string soundDirectory, ButtonDefinition? definition = null)
    {
        Text = definition is null ? "Add Button" : "Edit Button";
        ClientSize = new Size(650, 560);
        MinimumSize = new Size(560, 500);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;

        ConfigureBrowseButton(_target, BrowseTarget);
        ConfigureBrowseButton(_workingDirectory, BrowseDirectory);
        ConfigureBrowseButton(_iconPath, BrowseIcon);
        ConfigureBrowseButton(_soundFile, BrowseSound);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 15
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddField(layout, "Name", _name);
        AddField(layout, "Program, file, or URI (optional when a sound is selected)", _target);
        AddField(layout, "Arguments", _arguments);
        AddField(layout, "Working folder", _workingDirectory);
        AddField(layout, "Custom icon (optional)", _iconPath);
        AddField(layout, $"Sound file (WAV or MP3; relative paths use {DisplayDirectory(soundDirectory)})", _soundFile);
        _monitorFiles.Dock = DockStyle.Top;
        _monitorFiles.Margin = new Padding(0, 4, 0, 10);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(_monitorFiles);

        var hint = new LabelControl
        {
            Text = "A sound can play before the program starts. Leave the program blank to create a sound-only button.",
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Dock = DockStyle.Fill
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(hint);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 12, 0, 0)
        };
        var cancel = new SimpleButton { Text = "Cancel", DialogResult = DialogResult.Cancel, Size = new Size(90, 32) };
        var add = new SimpleButton
        {
            Text = definition is null ? "Add" : "Save",
            DialogResult = DialogResult.OK,
            Size = new Size(90, 32)
        };
        actions.Controls.Add(cancel);
        actions.Controls.Add(add);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(actions);
        Controls.Add(layout);

        AcceptButton = add;
        CancelButton = cancel;

        if (definition is not null)
        {
            _name.Text = definition.DisplayName;
            _target.Text = definition.LaunchTarget;
            _arguments.Text = definition.Arguments;
            _workingDirectory.Text = definition.WorkingDirectory;
            _iconPath.Text = definition.IconPath;
            _soundFile.Text = definition.SoundFile;
            _monitorFiles.Checked = definition.MonitorFileActivity;
        }

        FormClosing += ValidateBeforeClose;
    }

    public ButtonDefinition GetDefinition(long id = 0, int sortOrder = 0) => new(
        id, _name.Text.Trim(), _target.Text.Trim(), _arguments.Text.Trim(),
        _workingDirectory.Text.Trim(), _iconPath.Text.Trim(), _soundFile.Text.Trim(),
        _monitorFiles.Checked, sortOrder);

    private static void AddField(TableLayoutPanel layout, string label, Control editor)
    {
        var caption = new LabelControl { Text = label, Dock = DockStyle.Fill };
        editor.Dock = DockStyle.Top;
        editor.Margin = new Padding(0, 2, 0, 10);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(caption);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(editor);
    }

    private static void ConfigureBrowseButton(ButtonEdit editor, ButtonPressedEventHandler handler)
    {
        editor.Properties.Buttons[0].Caption = "Browse";
        editor.ButtonClick += handler;
    }

    private void BrowseTarget(object sender, ButtonPressedEventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Choose a program, shortcut, or launchable file",
            Filter = "Programs and shortcuts|*.exe;*.lnk;*.appref-ms;*.cmd;*.bat;*.url|All files|*.*"
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _target.Text = dialog.FileName;
            if (string.IsNullOrWhiteSpace(_name.Text))
            {
                _name.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
        }
    }

    private void BrowseDirectory(object sender, ButtonPressedEventArgs e)
    {
        using var dialog = new FolderBrowserDialog { Description = "Choose the program's working folder" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _workingDirectory.Text = dialog.SelectedPath;
        }
    }

    private void BrowseIcon(object sender, ButtonPressedEventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Choose a custom icon or image",
            Filter = "Icons and images|*.ico;*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*"
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _iconPath.Text = dialog.FileName;
        }
    }

    private void BrowseSound(object sender, ButtonPressedEventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Choose a button sound",
            Filter = "Sound files|*.wav;*.mp3|All files|*.*"
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _soundFile.Text = dialog.FileName;
            if (string.IsNullOrWhiteSpace(_name.Text))
            {
                _name.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
        }
    }

    private void ValidateBeforeClose(object? sender, FormClosingEventArgs e)
    {
        if (DialogResult != DialogResult.OK)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_name.Text))
        {
            XtraMessageBox.Show(this, "A button name is required.", "Incomplete button");
            e.Cancel = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(_target.Text) && string.IsNullOrWhiteSpace(_soundFile.Text))
        {
            XtraMessageBox.Show(this, "Choose a program, a sound, or both.", "Incomplete button");
            e.Cancel = true;
        }
    }

    private static string DisplayDirectory(string directory) =>
        string.IsNullOrWhiteSpace(directory) ? "the configured sound folder" : directory;
}
