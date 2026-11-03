using System.ComponentModel;
using System.Diagnostics;

namespace DVMS.StartTree;

internal static class LauncherService
{
    public static void Launch(MenuItemDefinition item)
    {
        if (item.IsFolder)
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = item.LaunchPath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(item.LaunchPath) ?? string.Empty
            });
        }
        catch (Exception exception) when (
            exception is Win32Exception or FileNotFoundException or InvalidOperationException)
        {
            MessageBox.Show(
                $"Start Tree could not launch '{item.DisplayName}'.\n\n{exception.Message}",
                "Unable to start program",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
