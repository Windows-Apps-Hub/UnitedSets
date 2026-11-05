using System.Runtime.InteropServices;
using UnitedSets.Helpers;

namespace UnitedSets.Services;

/// <summary>
/// A click-through, semi-transparent accent colored rectangle shown on top of everything while a window is dragged,
/// to show where it will be grouped (like Groupy's tab preview).
/// </summary>
/// <remarks>
/// Plain Win32 layered window: it must not take focus, must not receive input and must stay above the dragged window.
/// </remarks>
static class DropHighlightOverlay
{
    static nint hwnd;
    static WndProcDelegate? wndProc; // kept alive as long as the window exists
    static System.Drawing.Rectangle current;
    public static nint Handle => hwnd;

    public static void Show(System.Drawing.Rectangle rect)
    {
        try
        {
            EnsureCreated();
            if (hwnd == 0) return;
            if (rect == current && IsWindowVisible(hwnd)) return;
            current = rect;
            SetWindowPos(hwnd, HWND_TOPMOST, rect.X, rect.Y, rect.Width, rect.Height,
                SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }
        catch (Exception ex)
        {
            CrashLog.Write("Drop highlight", ex);
        }
    }

    public static void Hide()
    {
        if (hwnd == 0) return;
        current = default;
        ShowWindow(hwnd, SW_HIDE);
    }

    static void EnsureCreated()
    {
        if (hwnd != 0) return;
        var accent = new Windows.UI.ViewManagement.UISettings().GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
        wndProc = DefWindowProc;
        const string className = "UnitedSetsDropHighlight";
        var wc = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc),
            hInstance = GetModuleHandle(null),
            hbrBackground = CreateSolidBrush((uint)(accent.R | (accent.G << 8) | (accent.B << 16))),
            lpszClassName = className
        };
        RegisterClassEx(ref wc);
        hwnd = CreateWindowEx(
            WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE,
            className, "United Sets drop highlight", WS_POPUP,
            0, 0, 0, 0, 0, 0, wc.hInstance, 0);
        if (hwnd == 0) return;
        SetLayeredWindowAttributes(hwnd, 0, 96, LWA_ALPHA);
        // rounded corners on Windows 11
        int round = DWMWCP_ROUND;
        DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
        UnitedSetsApp.Current.RegisterUnitedSetsWindow(WindowEx.FromWindowHandle(hwnd));
    }

    const uint WS_POPUP = 0x80000000;
    const uint WS_EX_LAYERED = 0x80000, WS_EX_TRANSPARENT = 0x20, WS_EX_TOPMOST = 0x8, WS_EX_TOOLWINDOW = 0x80, WS_EX_NOACTIVATE = 0x8000000;
    const uint LWA_ALPHA = 0x2;
    const uint SWP_NOACTIVATE = 0x10, SWP_SHOWWINDOW = 0x40;
    const int SW_HIDE = 0;
    const int DWMWA_WINDOW_CORNER_PREFERENCE = 33, DWMWCP_ROUND = 2;
    static readonly nint HWND_TOPMOST = -1;
    delegate nint WndProcDelegate(nint hwnd, uint msg, nint wParam, nint lParam);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public nint hIconSm;
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "RegisterClassExW")]
    static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateWindowExW")]
    static extern nint CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam);
    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);
    [DllImport("user32.dll")]
    static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")]
    static extern bool ShowWindow(nint hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    static extern bool IsWindowVisible(nint hWnd);
    [DllImport("gdi32.dll")]
    static extern nint CreateSolidBrush(uint color);
    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int value, int size);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
    static extern nint GetModuleHandle(string? lpModuleName);
}
