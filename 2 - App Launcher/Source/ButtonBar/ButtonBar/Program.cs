using DevExpress.LookAndFeel;

namespace DVMS.ButtonBar;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        UserLookAndFeel.Default.SetSkinStyle("WXI");

        var database = new ButtonBarDatabase();
        database.Initialize();
        Application.ApplicationExit += (_, _) =>
        {
            if (FileActivityMonitor.Instance is IDisposable monitor) monitor.Dispose();
        };
        Application.Run(new ButtonBarForm(database));
    }
}
