using DevExpress.LookAndFeel;

namespace DVMS.FileManager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        UserLookAndFeel.Default.SetSkinStyle("WXI");

        var database = new FileManagerDatabase();
        database.Initialize();
        Application.Run(new FileManagerForm(database));
    }
}
