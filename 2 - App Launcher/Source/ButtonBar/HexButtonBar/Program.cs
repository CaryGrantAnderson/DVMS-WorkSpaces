using DevExpress.LookAndFeel;

namespace DVMS.ButtonBar;

internal static class HexButtonBarProgram
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        UserLookAndFeel.Default.SetSkinStyle("WXI");
        var database = new ButtonBarDatabase("HexButtonBar");
        database.Initialize();
        Application.ApplicationExit += (_, _) =>
        {
            if (FileActivityMonitor.Instance is IDisposable monitor) monitor.Dispose();
        };
        Application.Run(new ButtonBarForm(database, hexMode: true));
    }
}
