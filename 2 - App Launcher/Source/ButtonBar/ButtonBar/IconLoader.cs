using System.Runtime.InteropServices;

namespace DVMS.ButtonBar;

internal static class IconLoader
{
    private const uint ShgfiIcon = 0x000000100;
    private const uint ShgfiLargeIcon = 0x000000000;
    private const uint ShgfiUseFileAttributes = 0x000000010;
    private const uint FileAttributeNormal = 0x00000080;

    public static Image Load(ButtonDefinition definition, Size size)
    {
        var iconSource = string.IsNullOrWhiteSpace(definition.IconPath)
            ? definition.LaunchTarget
            : definition.IconPath;
        iconSource = Environment.ExpandEnvironmentVariables(iconSource);

        if (File.Exists(iconSource) && IsImage(iconSource))
        {
            using var source = Image.FromFile(iconSource);
            return new Bitmap(source, size);
        }

        var info = new ShellFileInfo();
        var flags = ShgfiIcon | ShgfiLargeIcon;
        var attributes = 0u;
        if (!File.Exists(iconSource))
        {
            flags |= ShgfiUseFileAttributes;
            attributes = FileAttributeNormal;
        }

        var result = SHGetFileInfo(iconSource, attributes, ref info, (uint)Marshal.SizeOf(info), flags);
        if (result != 0 && info.IconHandle != 0)
        {
            try
            {
                using var icon = Icon.FromHandle(info.IconHandle);
                return new Bitmap(icon.ToBitmap(), size);
            }
            finally
            {
                DestroyIcon(info.IconHandle);
            }
        }

        return SystemIcons.Application.ToBitmap();
    }

    private static bool IsImage(string path) =>
        new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico" }
            .Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct ShellFileInfo
    {
        public nint IconHandle;
        public int IconIndex;
        public uint Attributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string DisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string TypeName;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern nint SHGetFileInfo(
        string path, uint fileAttributes, ref ShellFileInfo info, uint infoSize, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(nint iconHandle);
}
