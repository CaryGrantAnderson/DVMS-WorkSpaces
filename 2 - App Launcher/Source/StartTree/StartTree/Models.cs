namespace DVMS.StartTree;

internal sealed record AppSettings(bool KeepMenuOpenAfterLaunch, int? IconX, int? IconY);

internal sealed record MenuItemDefinition(
    string ItemKey,
    string? ParentKey,
    string DisplayName,
    string LaunchPath,
    string Source,
    bool IsFolder,
    int SortOrder);
