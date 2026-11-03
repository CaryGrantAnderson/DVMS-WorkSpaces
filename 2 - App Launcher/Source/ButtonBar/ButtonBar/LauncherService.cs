using System.ComponentModel;
using System.Diagnostics;

namespace DVMS.ButtonBar;

internal static class LauncherService
{
    public static LaunchResult Launch(ButtonDefinition definition, string soundDirectory)
    {
        SoundService.Play(definition.SoundFile, soundDirectory);
        if (string.IsNullOrWhiteSpace(definition.LaunchTarget))
            return new LaunchResult(false, null);

        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ExpandEnvironmentVariables(definition.LaunchTarget),
                Arguments = definition.Arguments,
                WorkingDirectory = ResolveWorkingDirectory(definition),
                UseShellExecute = true
            });
            return new LaunchResult(true, process);
        }
        catch (Exception exception) when (
            exception is Win32Exception or FileNotFoundException or InvalidOperationException)
        {
            MessageBox.Show($"Button Bar could not start '{definition.DisplayName}'.\n\n{exception.Message}",
                "Unable to start program", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new LaunchResult(false, null);
        }
    }

    private static string ResolveWorkingDirectory(ButtonDefinition definition)
    {
        if (!string.IsNullOrWhiteSpace(definition.WorkingDirectory))
            return Environment.ExpandEnvironmentVariables(definition.WorkingDirectory);
        var expanded = Environment.ExpandEnvironmentVariables(definition.LaunchTarget);
        return Path.IsPathRooted(expanded) ? Path.GetDirectoryName(expanded) ?? string.Empty : string.Empty;
    }
}
