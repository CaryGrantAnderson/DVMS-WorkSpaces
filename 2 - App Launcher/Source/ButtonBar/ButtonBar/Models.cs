namespace DVMS.ButtonBar;

internal enum ButtonSize
{
    Small,
    Medium,
    Large
}

internal sealed record ButtonDefinition(
    long Id,
    string DisplayName,
    string LaunchTarget,
    string Arguments,
    string WorkingDirectory,
    string IconPath,
    string SoundFile,
    bool MonitorFileActivity,
    int SortOrder);

internal sealed record ButtonBarSettings(
    bool LockButtonEditing,
    bool LockWindowResize,
    bool LockWindowMovement,
    bool AlwaysOnTop,
    bool ShowButtonLabels,
    bool ShowMenuBar,
    ButtonSize ButtonSize,
    string SoundDirectory,
    int? WindowX,
    int? WindowY,
    int? WindowWidth,
    int? WindowHeight);

internal sealed record LaunchHistoryEntry(
    long Id,
    long? ButtonId,
    string DisplayName,
    string LaunchTarget,
    string Arguments,
    DateTimeOffset LaunchedAtLocal,
    DateTime LaunchedAtUtc,
    int? ProcessId,
    DateTimeOffset? EndedAtLocal,
    DateTime? EndedAtUtc,
    double? DurationSeconds,
    string TrackingStatus,
    int? ExitCode,
    string FileMonitoringStatus);

internal sealed record LaunchResult(bool Started, System.Diagnostics.Process? Process);

internal sealed record HexButtonAppearance(
    long ButtonId,
    bool ShowIcon,
    bool UseGradient,
    int StartColorArgb,
    int EndColorArgb,
    string GradientDirection,
    bool ShowLabel,
    float LabelFontSize,
    bool LabelBold,
    int? TextColorArgb)
{
    public static HexButtonAppearance Default(long buttonId) => new(
        buttonId, true, true, Color.FromArgb(28, 101, 125).ToArgb(),
        Color.FromArgb(10, 54, 72).ToArgb(), "Vertical", true, 8f, false, null);
}

internal sealed record FileActivityEntry(
    long Id,
    long LaunchHistoryId,
    int ProcessId,
    string ProcessName,
    string FilePath,
    string AccessType,
    DateTimeOffset OpenedAtLocal,
    DateTimeOffset? ClosedAtLocal,
    double? DurationSeconds,
    string TrackingStatus);
