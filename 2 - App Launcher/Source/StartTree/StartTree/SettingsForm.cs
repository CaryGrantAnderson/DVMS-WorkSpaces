using DevExpress.XtraEditors;

namespace DVMS.StartTree;

internal sealed class SettingsForm : XtraForm
{
    private readonly CheckEdit _keepOpen = new()
    {
        Text = "Keep the program tree open after launching an item",
        AutoSizeInLayoutControl = true
    };

    public SettingsForm(AppSettings settings)
    {
        Text = "Start Tree Settings";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(470, 150);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;

        _keepOpen.Checked = settings.KeepMenuOpenAfterLaunch;

        var description = new LabelControl
        {
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Text = "When this is off, Start Tree closes as soon as a program starts. " +
                   "It always closes when you click elsewhere.",
            Bounds = new Rectangle(24, 48, 420, 42)
        };

        var ok = new SimpleButton { Text = "OK", DialogResult = DialogResult.OK };
        var cancel = new SimpleButton { Text = "Cancel", DialogResult = DialogResult.Cancel };
        ok.SetBounds(274, 104, 82, 30);
        cancel.SetBounds(364, 104, 82, 30);
        _keepOpen.SetBounds(20, 16, 426, 28);

        Controls.AddRange(new Control[] { _keepOpen, description, ok, cancel });
        AcceptButton = ok;
        CancelButton = cancel;
    }

    public bool KeepMenuOpenAfterLaunch => _keepOpen.Checked;
}
