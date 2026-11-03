namespace DVMS.StartTree;

internal sealed class StartMenuDiscoveryService
{
    private static readonly HashSet<string> LaunchableExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".lnk", ".url", ".appref-ms", ".exe", ".cmd", ".bat" };

    public IReadOnlyCollection<MenuItemDefinition> Discover()
    {
        var discovered = new Dictionary<string, MenuItemDefinition>(StringComparer.OrdinalIgnoreCase);

        AddSource(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
            "All users",
            discovered);
        AddSource(
            Environment.GetFolderPath(Environment.SpecialFolder.Programs),
            "Current user",
            discovered);

        return discovered.Values.ToArray();
    }

    private static void AddSource(
        string root,
        string source,
        IDictionary<string, MenuItemDefinition> destination)
    {
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
        {
            return;
        }

        var sourceRootKey = MakeKey(source, string.Empty);
        destination[sourceRootKey] = new MenuItemDefinition(
            sourceRootKey, null, source, root, source, true, SourceSort(source));

        try
        {
            foreach (var directory in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(root, directory);
                var key = MakeKey(source, relative);
                destination[key] = new MenuItemDefinition(
                    key,
                    ParentKey(source, relative),
                    Path.GetFileName(directory),
                    directory,
                    source,
                    true,
                    0);
            }

            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                         .Where(IsLaunchable))
            {
                var relative = Path.GetRelativePath(root, file);
                var key = MakeKey(source, relative);
                destination[key] = new MenuItemDefinition(
                    key,
                    ParentKey(source, relative),
                    FriendlyName(file),
                    file,
                    source,
                    false,
                    1);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // A protected Start Menu subfolder should not prevent the launcher from opening.
        }
        catch (IOException)
        {
            // The Start Menu can change while it is being enumerated; use the entries found so far.
        }
    }

    private static bool IsLaunchable(string path) =>
        LaunchableExtensions.Contains(Path.GetExtension(path));

    private static string FriendlyName(string path) =>
        Path.GetFileNameWithoutExtension(path).Trim();

    private static string MakeKey(string source, string relativePath) =>
        $"{source}|{relativePath.Replace(Path.DirectorySeparatorChar, '/')}";

    private static string ParentKey(string source, string relativePath)
    {
        var parent = Path.GetDirectoryName(relativePath);
        return MakeKey(source, parent ?? string.Empty);
    }

    private static int SourceSort(string source) =>
        source.Equals("Current user", StringComparison.OrdinalIgnoreCase) ? 0 : 1;
}
