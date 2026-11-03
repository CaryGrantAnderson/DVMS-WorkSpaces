using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DVMS.ButtonBar;

internal static class ProcessTrackingService
{
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpNoZOrder = 0x0004;

    public static void Track(
        Process process,
        ButtonDefinition button,
        long historyId,
        ButtonBarDatabase database,
        Screen targetScreen)
    {
        var startedAt = DateTimeOffset.Now;
        var completed = 0;
        void CompleteOnce()
        {
            if (Interlocked.Exchange(ref completed, 1) == 0)
                Complete(process, button, historyId, database, startedAt);
        }
        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => CompleteOnce();
        _ = Task.Run(() => MoveMainWindowToScreen(process, targetScreen));

        if (process.HasExited)
        {
            CompleteOnce();
        }
    }

    private static void Complete(
        Process process,
        ButtonDefinition button,
        long historyId,
        ButtonBarDatabase database,
        DateTimeOffset startedAt)
    {
        try
        {
            var endedAt = DateTimeOffset.Now;
            var duration = Math.Max(0, (endedAt - startedAt).TotalSeconds);
            var status = duration < 2 && IsIndirectTarget(button.LaunchTarget)
                ? "Launcher exited"
                : "Completed";
            int? exitCode = null;
            try { exitCode = process.ExitCode; } catch (InvalidOperationException) { }
            database.CompleteLaunch(historyId, endedAt, duration, status, exitCode);
        }
        catch (InvalidOperationException)
        {
            // The process ended while its final details were being read.
        }
    }

    private static bool IsIndirectTarget(string target)
    {
        var extension = Path.GetExtension(target);
        return target.Contains(':') && !Path.IsPathRooted(target)
               || extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase)
               || extension.Equals(".url", StringComparison.OrdinalIgnoreCase)
               || extension.Equals(".appref-ms", StringComparison.OrdinalIgnoreCase);
    }

    private static void MoveMainWindowToScreen(Process process, Screen targetScreen)
    {
        try
        {
            try { process.WaitForInputIdle(5000); } catch (InvalidOperationException) { }
            nint window = 0;
            for (var attempt = 0; attempt < 50 && !process.HasExited; attempt++)
            {
                process.Refresh();
                window = process.MainWindowHandle;
                if (window != 0) break;
                Thread.Sleep(100);
            }
            if (window == 0 || Screen.FromHandle(window).DeviceName == targetScreen.DeviceName) return;
            if (!GetWindowRect(window, out var current)) return;

            var area = targetScreen.WorkingArea;
            var width = Math.Min(current.Right - current.Left, area.Width);
            var height = Math.Min(current.Bottom - current.Top, area.Height);
            var x = area.Left + Math.Max(0, (area.Width - width) / 2);
            var y = area.Top + Math.Max(0, (area.Height - height) / 2);
            SetWindowPos(window, 0, x, y, width, height, SwpNoActivate | SwpNoZOrder);
        }
        catch (InvalidOperationException)
        {
            // Short-lived launch proxies may exit before exposing their window.
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(nint window, out NativeRectangle rectangle);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        nint window, nint insertAfter, int x, int y, int width, int height, uint flags);
}
