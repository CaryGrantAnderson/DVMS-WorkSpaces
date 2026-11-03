using DevExpress.LookAndFeel;

namespace DVMS.StartTree;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        UserLookAndFeel.Default.SetSkinStyle("WXI");

        var database = new StartTreeDatabase();
        database.Initialize();

        var discovery = new StartMenuDiscoveryService();
        database.ReplaceMenuItems(discovery.Discover());

        Application.Run(new LauncherIconForm(database));
    }
}
