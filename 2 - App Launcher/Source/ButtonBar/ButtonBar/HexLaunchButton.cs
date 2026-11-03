using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace DVMS.ButtonBar;

internal sealed class HexLaunchButton : Control
{
    private bool _hovered;
    private bool _pressed;
    private bool _suppressClick;
    private readonly ToolTip _toolTip = new();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image? Icon { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HexButtonAppearance Appearance { get; set; } = HexButtonAppearance.Default(0);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ToolTipText { set => _toolTip.SetToolTip(this, value); }

    public HexLaunchButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.UserPaint, true);
        Cursor = Cursors.Hand;
        TabStop = true;
        Margin = new Padding(4);
        ForeColor = Color.White;
        Font = new Font((SystemFonts.MessageBoxFont ?? Control.DefaultFont).FontFamily, 9f);
        AccessibleRole = AccessibleRole.PushButton;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        using var path = CreatePath(ClientRectangle);
        Region?.Dispose();
        Region = new Region(path);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = Rectangle.Inflate(ClientRectangle, -2, -2);
        using var path = CreatePath(bounds);
        var top = Color.FromArgb(Appearance.StartColorArgb);
        var bottom = Appearance.UseGradient ? Color.FromArgb(Appearance.EndColorArgb) : top;
        if (_pressed) { top = Adjust(top, -.22f); bottom = Adjust(bottom, -.22f); }
        else if (_hovered) { top = Adjust(top, .15f); bottom = Adjust(bottom, .15f); }
        var direction = Enum.TryParse<LinearGradientMode>(Appearance.GradientDirection, out var parsed)
            ? parsed : LinearGradientMode.Vertical;
        using Brush fill = Appearance.UseGradient
            ? new LinearGradientBrush(bounds, top, bottom, direction)
            : new SolidBrush(top);
        var borderColor = _hovered || Focused ? Adjust(top, .42f) : Adjust(top, .23f);
        using var border = new Pen(borderColor, 2f);
        e.Graphics.FillPath(fill, path);
        e.Graphics.DrawPath(border, path);

        var labelHeight = string.IsNullOrEmpty(Text) ? 0 : Math.Max(20, Height / 4);
        if (Icon is not null)
        {
            var availableHeight = Height - labelHeight - 16;
            var side = Math.Min(Math.Min(Icon.Width, Icon.Height), Math.Max(16, availableHeight));
            var imageRect = new Rectangle((Width - side) / 2, 8 + (availableHeight - side) / 2, side, side);
            e.Graphics.DrawImage(Icon, imageRect);
        }
        if (!string.IsNullOrEmpty(Text))
        {
            var textRect = new Rectangle(Width / 6, Height - labelHeight - 7, Width * 2 / 3, labelHeight);
            var average = Color.FromArgb((top.R + bottom.R) / 2, (top.G + bottom.G) / 2, (top.B + bottom.B) / 2);
            var textColor = Appearance.TextColorArgb is int argb
                ? Color.FromArgb(argb) : ContrastColor(average);
            using var labelFont = new Font(Font.FontFamily, Appearance.LabelFontSize,
                Appearance.LabelBold ? FontStyle.Bold : FontStyle.Regular);
            TextRenderer.DrawText(e.Graphics, Text, labelFont, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }
    }

    private static Color ContrastColor(Color color) =>
        color.R * .299 + color.G * .587 + color.B * .114 > 150 ? Color.FromArgb(25, 25, 28) : Color.White;

    private static Color Adjust(Color color, float amount)
    {
        int Channel(byte value) => amount >= 0
            ? (int)Math.Round(value + (255 - value) * amount)
            : (int)Math.Round(value * (1 + amount));
        return Color.FromArgb(color.A, Math.Clamp(Channel(color.R), 0, 255),
            Math.Clamp(Channel(color.G), 0, 255), Math.Clamp(Channel(color.B), 0, 255));
    }

    protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hovered = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        _suppressClick = e.Button != MouseButtons.Left;
        if (!_suppressClick) _pressed = true;
        Invalidate();
        base.OnMouseDown(e);
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        _pressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }
    protected override void OnClick(EventArgs e)
    {
        // WinForms Control can raise Click for either mouse button. A right click
        // belongs exclusively to the edit popup; keyboard activation has no held button.
        if (_suppressClick) return;
        base.OnClick(e);
    }
    protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
    protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Enter or Keys.Space)
        {
            _suppressClick = false;
            OnClick(EventArgs.Empty);
            e.Handled = true;
        }
        base.OnKeyDown(e);
    }

    private static GraphicsPath CreatePath(Rectangle rectangle)
    {
        var path = new GraphicsPath();
        if (rectangle.Width <= 0 || rectangle.Height <= 0) return path;
        var shoulder = rectangle.Height * .25f;
        path.AddPolygon([
            new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Top),
            new PointF(rectangle.Right, rectangle.Top + shoulder),
            new PointF(rectangle.Right, rectangle.Bottom - shoulder),
            new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Bottom),
            new PointF(rectangle.Left, rectangle.Bottom - shoulder),
            new PointF(rectangle.Left, rectangle.Top + shoulder)
        ]);
        path.CloseFigure();
        return path;
    }
}

internal sealed class HexFlowPanel : FlowLayoutPanel
{
    protected override void OnLayout(LayoutEventArgs levent)
    {
        var sample = Controls.Cast<Control>().FirstOrDefault(c => c.Visible);
        if (sample is null) { base.OnLayout(levent); return; }
        var cellWidth = sample.Width;
        var rowStep = Math.Max(1, (int)Math.Round(sample.Height * .75));
        var available = Math.Max(cellWidth, ClientSize.Width - Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
        var columns = Math.Max(1, available / cellWidth);
        // When wrapping is needed, reserve the extra half-cell used by offset rows.
        if (Controls.Count > columns && columns > 1 && columns * cellWidth + cellWidth / 2 > available)
            columns--;
        var index = 0;
        foreach (Control control in Controls)
        {
            if (!control.Visible) continue;
            var row = index / columns;
            var column = index % columns;
            var offset = row % 2 == 1 ? cellWidth / 2 : 0;
            control.Location = new Point(Padding.Left + column * cellWidth + offset,
                Padding.Top + row * rowStep);
            index++;
        }
        AutoScrollMinSize = new Size(columns * cellWidth + cellWidth / 2,
            Padding.Top + ((Math.Max(0, index - 1) / columns) + 1) * rowStep + sample.Height / 4 + Padding.Bottom);
    }
}
