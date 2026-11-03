using DevExpress.XtraEditors;

namespace DVMS.FileManager;

internal sealed class MetadataEditorForm : XtraForm
{
    private readonly MemoEdit _note = new() { Dock = DockStyle.Fill };
    private readonly TextEdit _description = new() { Dock = DockStyle.Fill };
    private readonly ComboBoxEdit _color = new() { Dock = DockStyle.Fill };

    public MetadataEditorForm(ItemMetadata metadata)
    {
        Text = $"Notes and appearance - {Path.GetFileName(metadata.Path)}";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(560, 420);
        Size = new Size(680, 520);

        _note.Text = metadata.Note;
        _description.Text = metadata.Description;
        _color.Properties.Items.AddRange(new object[] { "", "Red", "Orange", "Gold", "Green", "Blue", "Purple", "Gray" });
        _color.Text = metadata.ColorName;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 1, RowCount = 7 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new LabelControl { Text = "Description", AutoSizeMode = LabelAutoSizeMode.Vertical }, 0, 0);
        layout.Controls.Add(_description, 0, 1);
        layout.Controls.Add(new LabelControl { Text = "Notes", AutoSizeMode = LabelAutoSizeMode.Vertical, Padding = new Padding(0, 12, 0, 0) }, 0, 2);
        layout.Controls.Add(_note, 0, 3);
        layout.Controls.Add(new LabelControl { Text = "Color marker", Padding = new Padding(0, 12, 0, 0) }, 0, 4);
        layout.Controls.Add(_color, 0, 5);

        var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 12, 0, 0) };
        var save = new SimpleButton { Text = "Save", DialogResult = DialogResult.OK };
        var cancel = new SimpleButton { Text = "Cancel", DialogResult = DialogResult.Cancel };
        buttons.Controls.Add(save);
        buttons.Controls.Add(cancel);
        layout.Controls.Add(buttons, 0, 6);
        Controls.Add(layout);
        AcceptButton = save;
        CancelButton = cancel;
    }

    public ItemMetadata Build(string path) => new(path, _note.Text.Trim(), _description.Text.Trim(), _color.Text.Trim());
}
