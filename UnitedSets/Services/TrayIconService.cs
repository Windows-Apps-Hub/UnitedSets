using System.IO;
using System.Runtime.InteropServices;
using UnitedSets.Configurations;
using UnitedSets.Helpers;

namespace UnitedSets.Services;

/// <summary>
/// Notification area (tray) icon. United Sets keeps running in the background when every set window is closed
/// (like Groupy), so windows can still be grouped by dragging them onto each other.
/// <list type="bullet">
/// <item>Left click: open a new set window</item>
/// <item>Right click: menu (new set, settings, exit)</item>
/// </list>
/// </summary>
static class TrayIconService
{
    const uint WM_APP_TRAY = 0x8000 + 0x55; // WM_APP + n
    const uint WM_LBUTTONUP = 0x0202, WM_RBUTTONUP = 0x0205, WM_CONTEXTMENU = 0x007B, WM_NULL = 0;
    const uint NIM_ADD = 0, NIM_MODIFY = 1, NIM_DELETE = 2, NIM_SETVERSION = 4;
    const uint NIF_MESSAGE = 1, NIF_ICON = 2, NIF_TIP = 4, NIF_INFO = 0x10;
    const uint NIIF_INFO = 1;
    const uint NOTIFYICON_VERSION_4 = 4;
    const int CmdNewSet = 1, CmdSettings = 2, CmdExit = 3;
    static nint window;
    static WndProcDelegate? wndProc;
    static uint taskbarCreatedMessage;
    static bool added;

    public static void Start()
    {
        if (window != 0) return;
        try
        {
            wndProc = WndProc;
            const string className = "UnitedSetsTrayWindow";
            var wc = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc),
                hInstance = GetModuleHandle(null),
                lpszClassName = className
            };
            RegisterClassEx(ref wc);
            // a real (hidden) top-level window: the context menu needs a window that can be foreground
            window = CreateWindowEx(0, className, "United Sets", 0x80000000 /* WS_POPUP */, 0, 0, 0, 0, 0, 0, wc.hInstance, 0);
            if (window == 0) return;
            UnitedSetsApp.Current.RegisterUnitedSetsWindow(WindowEx.FromWindowHandle(window));
            // re-add the icon when Explorer restarts
            taskbarCreatedMessage = RegisterWindowMessage("TaskbarCreated");
            AddIcon();
        }
        catch (Exception ex)
        {
            CrashLog.Write("Tray icon", ex);
        }
    }

    /// <summary>
    /// Shows a notification from the tray icon
    /// </summary>
    public static void ShowBalloon(string title, string text)
    {
        if (!added) return;
        var data = NewData();
        data.uFlags = NIF_INFO;
        data.szInfoTitle = title;
        data.szInfo = text;
        data.dwInfoFlags = NIIF_INFO;
        Shell_NotifyIcon(NIM_MODIFY, ref data);
    }

    public static void Stop()
    {
        if (!added) return;
        var data = NewData();
        Shell_NotifyIcon(NIM_DELETE, ref data);
        added = false;
    }

    static NOTIFYICONDATA NewData() => new()
    {
        cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
        hWnd = window,
        uID = 1
    };

    static void AddIcon()
    {
        var data = NewData();
        data.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
        data.uCallbackMessage = WM_APP_TRAY;
        data.hIcon = LoadAppIcon();
        data.szTip = Apps.Constants.AppName;
        added = Shell_NotifyIcon(NIM_ADD, ref data);
        data.uVersion = NOTIFYICON_VERSION_4;
        Shell_NotifyIcon(NIM_SETVERSION, ref data);
    }

    static nint LoadAppIcon()
    {
        var ico = UnitedSetsApp.Current.Configuration.MainConfiguration.TaskbarIco ?? "Assets/UnitedSets.ico";
        var path = Path.IsPathRooted(ico) ? ico : Path.Combine(USConfig.RootLocation, ico);
        if (!File.Exists(path)) path = Path.Combine(USConfig.RootLocation, "Assets", "UnitedSets.ico");
        var small = GetSystemMetrics(49 /* SM_CXSMICON */);
        return LoadImage(0, path, 1 /* IMAGE_ICON */, small, small, 0x10 /* LR_LOADFROMFILE */);
    }

    static nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        try
        {
            if (msg == WM_APP_TRAY)
            {
                // with NOTIFYICON_VERSION_4 the event is in the low word of lParam
                var ev = (uint)(lParam & 0xFFFF);
                if (ev is WM_LBUTTONUP)
                    UnitedSetsApp.Current.DispatcherQueue.TryEnqueue(OpenOrActivate);
                else if (ev is WM_RBUTTONUP or WM_CONTEXTMENU)
                    ShowMenu();
                return 0;
            }
            if (msg == taskbarCreatedMessage && taskbarCreatedMessage != 0)
            {
                AddIcon();
                return 0;
            }
        }
        catch (Exception ex)
        {
            CrashLog.Write("Tray icon", ex);
        }
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    static void OpenOrActivate()
    {
        if (UnitedSetsApp.Current.LastActiveWindow is { } w)
            w.Activate();
        else
            UnitedSetsApp.Current.CreateNewSetWindow();
    }

    static void ShowMenu()
    {
        var menu = CreatePopupMenu();
        AppendMenu(menu, 0, CmdNewSet, "New set window");
        AppendMenu(menu, 0, CmdSettings, "Settings");
        AppendMenu(menu, 0x800 /* MF_SEPARATOR */, 0, null);
        AppendMenu(menu, 0, CmdExit, "Exit (release all windows)");
        GetCursorPos(out var pt);
        // required so the menu closes when clicking elsewhere
        SetForegroundWindow(window);
        var cmd = TrackPopupMenu(menu, 0x100 /* TPM_RETURNCMD */ | 0x20 /* TPM_BOTTOMALIGN */, pt.X, pt.Y, 0, window, 0);
        PostMessage(window, WM_NULL, 0, 0);
        DestroyMenu(menu);
        var app = UnitedSetsApp.Current;
        switch (cmd)
        {
            case CmdNewSet:
                app.DispatcherQueue.TryEnqueue(() => app.CreateNewSetWindow());
                break;
            case CmdSettings:
                app.DispatcherQueue.TryEnqueue(() => app.Settings.LaunchSettings(app.MainWindow));
                break;
            case CmdExit:
                app.DispatcherQueue.TryEnqueue(() => app.ExitReleasingAllWindows());
                break;
        }
    }

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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct NOTIFYICONDATA
    {
        public uint cbSize;
        public nint hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public nint hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint dwInfoFlags;
        public Guid guidItem;
        public nint hBalloonIcon;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct POINT { public int X, Y; }
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "Shell_NotifyIconW")]
    static extern bool Shell_NotifyIcon(uint message, ref NOTIFYICONDATA data);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "RegisterClassExW")]
    static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateWindowExW")]
    static extern nint CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam);
    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterWindowMessageW")]
    static extern uint RegisterWindowMessage(string name);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadImageW")]
    static extern nint LoadImage(nint hInst, string name, uint type, int cx, int cy, uint fuLoad);
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int index);
    [DllImport("user32.dll")]
    static extern nint CreatePopupMenu();
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "AppendMenuW")]
    static extern bool AppendMenu(nint menu, uint flags, nint id, string? text);
    [DllImport("user32.dll")]
    static extern int TrackPopupMenu(nint menu, uint flags, int x, int y, int reserved, nint hwnd, nint rect);
    [DllImport("user32.dll")]
    static extern bool DestroyMenu(nint menu);
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT pt);
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(nint hwnd);
    [DllImport("user32.dll")]
    static extern bool PostMessage(nint hwnd, uint msg, nint wParam, nint lParam);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
    static extern nint GetModuleHandle(string? lpModuleName);
}
