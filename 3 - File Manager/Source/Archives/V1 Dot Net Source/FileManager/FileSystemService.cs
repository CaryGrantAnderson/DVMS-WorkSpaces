using System.Diagnostics;

namespace DVMS.FileManager;

internal static class FileSystemService
{
    public static IReadOnlyList<FileSystemItem> Enumerate(string directory, FileManagerDatabase database)
    {
        var entries = new List<(FileSystemInfo Info, bool IsDirectory)>();
        try
        {
            entries.AddRange(new DirectoryInfo(directory).EnumerateDirectories().Select(x => ((FileSystemInfo)x, true)));
            entries.AddRange(new DirectoryInfo(directory).EnumerateFiles().Select(x => ((FileSystemInfo)x, false)));
        }
        catch (UnauthorizedAccessException) { throw new IOException("Access to this folder was denied."); }

        var metadata = database.GetMetadata(entries.Select(x => x.Info.FullName));
        return entries.Select(entry =>
        {
            metadata.TryGetValue(entry.Info.FullName, out var meta);
            var file = entry.Info as FileInfo;
            return new FileSystemItem(
                entry.Info.Name,
                entry.Info.FullName,
                entry.IsDirectory,
                file?.Length,
                entry.Info.LastWriteTime,
                entry.Info.CreationTime,
                entry.IsDirectory ? "File folder" : DescribeType(file!.Extension),
                meta?.Note ?? "",
                meta?.Description ?? "",
                meta?.ColorName ?? "");
        }).OrderByDescending(x => x.IsDirectory).ThenBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    public static void Open(string path)
    {
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    public static string UniqueDestination(string destinationDirectory, string sourcePath)
    {
        var name = Path.GetFileName(sourcePath.TrimEnd(Path.DirectorySeparatorChar));
        var candidate = Path.Combine(destinationDirectory, name);
        if (!File.Exists(candidate) && !Directory.Exists(candidate)) return candidate;
        var stem = Path.GetFileNameWithoutExtension(name);
        var extension = Path.GetExtension(name);
        for (var i = 2; ; i++)
        {
            candidate = Path.Combine(destinationDirectory, $"{stem} ({i}){extension}");
            if (!File.Exists(candidate) && !Directory.Exists(candidate)) return candidate;
        }
    }

    public static void Copy(string source, string destination)
    {
        if (File.Exists(source)) { File.Copy(source, destination); return; }
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source)) File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
        foreach (var child in Directory.EnumerateDirectories(source)) Copy(child, Path.Combine(destination, Path.GetFileName(child)));
    }

    private static string DescribeType(string extension) => string.IsNullOrWhiteSpace(extension)
        ? "File"
        : $"{extension.TrimStart('.').ToUpperInvariant()} file";
}
