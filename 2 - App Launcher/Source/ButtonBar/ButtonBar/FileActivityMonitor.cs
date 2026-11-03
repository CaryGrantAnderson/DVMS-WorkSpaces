using System.Collections.Concurrent;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace DVMS.ButtonBar;

internal sealed class FileActivityMonitor : IDisposable
{
    private static readonly string SessionName = $"DVMS-ButtonBar-FileActivity-{Environment.ProcessId}";
    private static readonly Lazy<FileActivityMonitor> LazyInstance = new(() => new FileActivityMonitor());
    private readonly ConcurrentDictionary<int, TrackingContext> _processes = new();
    private readonly ConcurrentDictionary<(long HistoryId, int ProcessId, ulong FileObject), OpenFile> _openFiles = new();
    private readonly object _startLock = new();
    private TraceEventSession? _session;
    private Task? _processingTask;

    public static FileActivityMonitor Instance => LazyInstance.Value;

    public bool Prepare(out string status)
    {
        try
        {
            EnsureStarted();
            status = "Ready";
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            status = "Unavailable - run Button Bar as administrator";
            return false;
        }
        catch (Exception exception)
        {
            status = $"Unavailable - {exception.Message}";
            return false;
        }
    }

    public void Track(int processId, long historyId, ButtonBarDatabase database)
    {
        _processes[processId] = new TrackingContext(historyId, database);
    }

    public void Dispose()
    {
        _session?.Dispose();
        try { _processingTask?.Wait(1000); } catch (AggregateException) { }
    }

    private void EnsureStarted()
    {
        if (_session is not null) return;
        lock (_startLock)
        {
            if (_session is not null) return;
            if (TraceEventSession.IsElevated() != true)
                throw new UnauthorizedAccessException("Kernel file monitoring requires administrator access.");

            _session = new TraceEventSession(SessionName) { StopOnDispose = true };
            _session.EnableKernelProvider(
                KernelTraceEventParser.Keywords.Process
                | KernelTraceEventParser.Keywords.FileIO
                | KernelTraceEventParser.Keywords.FileIOInit);

            var kernel = _session.Source.Kernel;
            kernel.ProcessStart += data => OnProcessStart(data.ProcessID, data.ParentID);
            kernel.ProcessStop += data => OnProcessStop(data.ProcessID);
            kernel.FileIOCreate += data => OnOpen(
                data.ProcessID, data.ProcessName, data.FileName, data.FileObject);
            kernel.FileIORead += data => MarkAccess(data.ProcessID, data.FileObject, read: true);
            kernel.FileIOWrite += data => MarkAccess(data.ProcessID, data.FileObject, write: true);
            kernel.FileIOClose += data => OnClose(data.ProcessID, data.FileObject);

            _processingTask = Task.Run(() => _session.Source.Process());
        }
    }

    private void OnProcessStart(int processId, int parentId)
    {
        if (_processes.TryGetValue(parentId, out var parent))
            _processes[processId] = parent;
    }

    private void OnProcessStop(int processId)
    {
        if (!_processes.TryRemove(processId, out _)) return;
        var keys = _openFiles.Keys.Where(key => key.ProcessId == processId).ToArray();
        foreach (var key in keys) Close(key);
    }

    private void OnOpen(int processId, string processName, string filePath, ulong fileObject)
    {
        if (fileObject == 0 || !_processes.TryGetValue(processId, out var context) || !ShouldTrack(filePath))
            return;

        try
        {
            var openedAt = DateTimeOffset.Now;
            var activityId = context.Database.OpenFileActivity(
                context.HistoryId, processId, processName, filePath, openedAt);
            _openFiles.TryAdd(
                (context.HistoryId, processId, fileObject),
                new OpenFile(activityId, context.Database, openedAt));
        }
        catch (Exception)
        {
            // File tracing must never interrupt the launched application or Button Bar.
        }
    }

    private void MarkAccess(int processId, ulong fileObject, bool read = false, bool write = false)
    {
        if (!_processes.TryGetValue(processId, out var context)) return;
        if (_openFiles.TryGetValue((context.HistoryId, processId, fileObject), out var openFile))
        {
            if (read) openFile.WasRead = true;
            if (write) openFile.WasWritten = true;
        }
    }

    private void OnClose(int processId, ulong fileObject)
    {
        if (!_processes.TryGetValue(processId, out var context)) return;
        Close((context.HistoryId, processId, fileObject));
    }

    private void Close((long HistoryId, int ProcessId, ulong FileObject) key)
    {
        if (!_openFiles.TryRemove(key, out var openFile)) return;
        try
        {
            var access = openFile.WasRead && openFile.WasWritten ? "Read / Write"
                : openFile.WasWritten ? "Write"
                : openFile.WasRead ? "Read"
                : "Open";
            openFile.Database.CloseFileActivity(
                openFile.ActivityId, openFile.OpenedAt, DateTimeOffset.Now, access);
        }
        catch (Exception)
        {
            // Keep the ETW consumer alive even if an individual database update fails.
        }
    }

    private static bool ShouldTrack(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path)) return false;
        var expanded = Path.GetFullPath(path);
        var excludedRoots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        };
        return excludedRoots
            .Where(root => !string.IsNullOrWhiteSpace(root))
            .All(root => !expanded.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
    }

    private sealed record TrackingContext(long HistoryId, ButtonBarDatabase Database);

    private sealed class OpenFile(long activityId, ButtonBarDatabase database, DateTimeOffset openedAt)
    {
        public long ActivityId { get; } = activityId;
        public ButtonBarDatabase Database { get; } = database;
        public DateTimeOffset OpenedAt { get; } = openedAt;
        public bool WasRead { get; set; }
        public bool WasWritten { get; set; }
    }
}
