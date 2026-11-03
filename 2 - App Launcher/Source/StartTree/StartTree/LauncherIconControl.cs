using System.Drawing.Drawing2D;

namespace DVMS.StartTree;

internal sealed class LauncherIconControl : Control
{
    public LauncherIconControl()
    {
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
        AccessibleName = "DVMS Start Tree";
        AccessibleDescription = "Opens the Windows programs tree";
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var bounds = Rectangle.Inflate(ClientRectangle, -5, -5);
        using var shadow = new SolidBrush(Color.FromArgb(55, Color.Black));
        e.Graphics.FillEllipse(shadow, bounds.X + 2, bounds.Y + 3, bounds.Width, bounds.Height);

        using var background = new LinearGradientBrush(
            bounds,
            Color.FromArgb(44, 125, 225),
            Color.FromArgb(20, 65, 145),
            45f);
        e.Graphics.FillEllipse(background, bounds);
        using var outline = new Pen(Color.FromArgb(175, 225, 255), 2f);
        e.Graphics.DrawEllipse(outline, bounds);

        var centerX = bounds.Left + bounds.Width / 2;
        var top = bounds.Top + 14;
        using var treePen = new Pen(Color.White, 3f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
        e.Graphics.DrawLine(treePen, centerX, top, centerX, bounds.Bottom - 14);
        e.Graphics.DrawLine(treePen, centerX, top + 10, centerX - 12, top + 18);
        e.Graphics.DrawLine(treePen, centerX, top + 19, centerX + 12, top + 27);

        using var nodeBrush = new SolidBrush(Color.White);
        foreach (var point in new[]
                 {
                     new Point(centerX, top),
                     new Point(centerX - 12, top + 18),
                     new Point(centerX + 12, top + 27),
                     new Point(centerX, bounds.Bottom - 14)
                 })
        {
            e.Graphics.FillEllipse(nodeBrush, point.X - 4, point.Y - 4, 8, 8);
        }
    }
}
