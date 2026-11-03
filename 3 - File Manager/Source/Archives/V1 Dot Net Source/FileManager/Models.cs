namespace DVMS.FileManager;

internal sealed record FileSystemItem(
    string Name,
    string FullPath,
    bool IsDirectory,
    long? Size,
    DateTime Modified,
    DateTime Created,
    string Type,
    string Note,
    string Description,
    string ColorName);

internal sealed record ItemMetadata(string Path, string Note, string Description, string ColorName);

internal sealed record FavoriteLocation(long Id, string Name, string Path, int SortOrder);

internal enum ClipboardOperation { Copy, Move }
