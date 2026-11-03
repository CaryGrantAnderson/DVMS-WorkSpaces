using DevExpress.XtraEditors;

namespace DVMS.ButtonBar;

internal sealed class HexAppearanceForm : XtraForm
{
    private readonly CheckEdit _showIcon = new() { Text = "Show icon" };
    private readonly CheckEdit _useGradient = new() { Text = "Use gradient fill" };
    private readonly CheckEdit _showLabel = new() { Text = "Show name at bottom" };
    private readonly CheckEdit _bold = new() { Text = "Bold name" };
    private readonly CheckEdit _automaticText = new() { Text = "Choose name color automatically" };
    private readonly SimpleButton _startColor = new() { Text = "Choose..." };
    private readonly SimpleButton _endColor = new() { Text = "Choose..." };
    private readonly SimpleButton _textColor = new() { Text = "Choose..." };
    private readonly ComboBoxEdit _direction = new();
    private readonly SpinEdit _fontSize = new();
    private readonly HexLaunchButton _preview;
    private readonly long _buttonId;
    private Color _start;
    private Color _end;
    private Color _text = Color.White;

    public HexAppearanceForm(string buttonName, HexButtonAppearance appearance)
    {
        _buttonId = appearance.ButtonId;
        _start = Color.FromArgb(appearance.StartColorArgb);
        _end = Color.FromArgb(appearance.EndColorArgb);
        if (appearance.TextColorArgb is int textArgb) _text = Color.FromArgb(textArgb);

        Text = $"Appearance - {buttonName}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(470, 590);

        _preview = new HexLaunchButton { Text = buttonName, Size = new Size(126, 140) };
        _showIcon.Checked = appearance.ShowIcon;
        _useGradient.Checked = appearance.UseGradient;
        _showLabel.Checked = appearance.ShowLabel;
        _bold.Checked = appearance.LabelBold;
        _automaticText.Checked = appearance.TextColorArgb is null;
        _fontSize.Properties.MinValue = 6;
        _fontSize.Properties.MaxValue = 14;
        _fontSize.Properties.Increment = .5m;
        _fontSize.EditValue = (decimal)appearance.LabelFontSize;
        _direction.Properties.Items.AddRange(new[] { "Vertical", "Horizontal", "ForwardDiagonal", "BackwardDiagonal" });
        _direction.SelectedItem = appearance.GradientDirection;
        if (_direction.SelectedIndex < 0) _direction.SelectedIndex = 0;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 2,
            RowCount = 0
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        AddRow(layout, "Preview", _preview, 145);
        AddRow(layout, "", _showIcon);
        AddRow(layout, "", _useGradient);
        AddRow(layout, "Surface color", _startColor);
        AddRow(layout, "Second gradient color", _endColor);
        AddRow(layout, "Gradient direction", _direction);
        AddRow(layout, "", _showLabel);
        AddRow(layout, "Name font size", _fontSize);
        AddRow(layout, "", _bold);
        AddRow(layout, "", _automaticText);
        AddRow(layout, "Name color", _textColor);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        var ok = new SimpleButton { Text = "OK", DialogResult = DialogResult.OK, Size = new Size(90, 30) };
        var cancel = new SimpleButton { Text = "Cancel", DialogResult = DialogResult.Cancel, Size = new Size(90, 30) };
        buttons.Controls.Add(ok);
        buttons.Controls.Add(cancel);
        Controls.Add(layout);
        Controls.Add(buttons);
        AcceptButton = ok;
        CancelButton = cancel;

        _startColor.Click += (_, _) => PickColor(ref _start);
        _endColor.Click += (_, _) => PickColor(ref _end);
        _textColor.Click += (_, _) => PickColor(ref _text);
        foreach (var editor in new BaseEdit[] { _showIcon, _useGradient, _showLabel, _bold, _automaticText, _fontSize, _direction })
            editor.EditValueChanged += (_, _) => RefreshPreview();
        RefreshPreview();
    }

    public HexButtonAppearance SelectedAppearance => new(_buttonId, _showIcon.Checked, _useGradient.Checked,
        _start.ToArgb(), _end.ToArgb(), _direction.Text, _showLabel.Checked,
        Convert.ToSingle(_fontSize.Value), _bold.Checked,
        _automaticText.Checked ? null : _text.ToArgb());

    private void PickColor(ref Color value)
    {
        using var dialog = new ColorDialog { Color = value, FullOpen = true };
        if (dialog.ShowDialog(this) == DialogResult.OK) { value = dialog.Color; RefreshPreview(); }
    }

    private void RefreshPreview()
    {
        _endColor.Enabled = _useGradient.Checked;
        _direction.Enabled = _useGradient.Checked;
        _textColor.Enabled = !_automaticText.Checked;
        _fontSize.Enabled = _showLabel.Checked;
        _bold.Enabled = _showLabel.Checked;
        _preview.Text = _showLabel.Checked ? Text.Replace("Appearance - ", "") : string.Empty;
        _preview.Appearance = SelectedAppearance;
        _preview.Invalidate();
        _startColor.Appearance.BackColor = _start;
        _endColor.Appearance.BackColor = _end;
        _textColor.Appearance.BackColor = _text;
    }

    private static void AddRow(TableLayoutPanel layout, string label, Control control, int height = 31)
    {
        var row = layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        if (!string.IsNullOrEmpty(label)) layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        layout.Controls.Add(control, 1, row);
    }
}
