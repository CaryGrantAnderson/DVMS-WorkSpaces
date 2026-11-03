using System.Runtime.InteropServices;

namespace DVMS.StartTree;

internal sealed class OutsideClickMonitor : IDisposable
{
    private const int WhMouseLowLevel = 14;
    private const int WmLeftButtonDown = 0x0201;
    private const int WmRightButtonDown = 0x0204;
    private const int WmMiddleButtonDown = 0x0207;

    private readonly Func<Rectangle> _popupBounds;
    private readonly Func<Rectangle?> _anchorBounds;
    private readonly Action _outsideClick;
    private readonly HookProcedure _procedure;
    private nint _hook;

    public OutsideClickMonitor(
        Func<Rectangle> popupBounds,
        Func<Rectangle?> anchorBounds,
        Action outsideClick)
    {
        _popupBounds = popupBounds;
        _anchorBounds = anchorBounds;
        _outsideClick = outsideClick;
        _procedure = HookCallback;
    }

    public void Start()
    {
        if (_hook == 0)
        {
            _hook = SetWindowsHookEx(WhMouseLowLevel, _procedure, 0, 0);
        }
    }

    public void Stop()
    {
        if (_hook != 0)
        {
            UnhookWindowsHookEx(_hook);
            _hook = 0;
        }
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    private nint HookCallback(int code, nint message, nint data)
    {
        if (code >= 0 && IsMouseDown(message))
        {
            var hookData = Marshal.PtrToStructure<LowLevelMouseHookData>(data);
            var point = new Point(hookData.Point.X, hookData.Point.Y);
            var anchor = _anchorBounds();
            if (!_popupBounds().Contains(point) && !(anchor?.Contains(point) ?? false))
            {
                _outsideClick();
            }
        }

        return CallNextHookEx(_hook, code, message, data);
    }

    private static bool IsMouseDown(nint message) =>
        message == WmLeftButtonDown || message == WmRightButtonDown || message == WmMiddleButtonDown;

    private delegate nint HookProcedure(int code, nint message, nint data);

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct NativePoint
    {
        public readonly int X;
        public readonly int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct LowLevelMouseHookData
    {
        public readonly NativePoint Point;
        public readonly uint MouseData;
        public readonly uint Flags;
        public readonly uint Time;
        public readonly nuint ExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(
        int hookId,
        HookProcedure procedure,
        nint moduleHandle,
        uint threadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hook);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hook, int code, nint message, nint data);
}
