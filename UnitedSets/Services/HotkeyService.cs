using System.Runtime.InteropServices;
using UnitedSets.Helpers;

namespace UnitedSets.Services;

/// <summary>
/// Global hotkeys to switch tabs of the set the foreground window belongs to (like Groupy):
/// <list type="bullet">
/// <item><c>Win + `</c>: next tab</item>
/// <item><c>Win + Shift + `</c>: previous tab</item>
/// </list>
/// A message-only window receives <c>WM_HOTKEY</c>, so the hotkeys keep working whichever set window is open.
/// </summary>
static class HotkeyService
{
    const int IdNextTab = 0x5501, IdPreviousTab = 0x5502;
    const uint MOD_SHIFT = 0x4, MOD_WIN = 0x8, MOD_NOREPEAT = 0x4000;
    const uint VK_OEM_3 = 0xC0; // ` ~
    const uint WM_HOTKEY = 0x0312;
    static nint messageWindow;
    // must be kept alive as long as the window exists
    static WndProcDelegate? wndProc;
    static bool registered;

    public static void Start()
    {
        if (messageWindow != 0) return;
        try
        {
            wndProc = WndProc;
            var className = "UnitedSetsHotkeyWindow";
            var wc = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc),
                hInstance = GetModuleHandle(null),
                lpszClassName = className
            };
            RegisterClassEx(ref wc);
            messageWindow = CreateWindowEx(0, className, "United Sets Hotkeys", 0, 0, 0, 0, 0, HWND_MESSAGE, 0, wc.hInstance, 0);
            if (messageWindow == 0)
            {
                CrashLog.Write("Hotkeys", new System.ComponentModel.Win32Exception());
                return;
            }
            Register(UnitedSetsApp.Current.Settings.TabSwitchHotkeys.Value);
            UnitedSetsApp.Current.Settings.TabSwitchHotkeys.PropertyChanged += (_, _) =>
                Register(UnitedSetsApp.Current.Settings.TabSwitchHotkeys.Value);
        }
        catch (Exception ex)
        {
            CrashLog.Write("Hotkeys", ex);
        }
    }

    static void Register(bool enabled)
    {
        if (messageWindow == 0) return;
        if (registered)
        {
            UnregisterHotKey(messageWindow, IdNextTab);
            UnregisterHotKey(messageWindow, IdPreviousTab);
            registered = false;
        }
        if (!enabled) return;
        var ok1 = RegisterHotKey(messageWindow, IdNextTab, MOD_WIN | MOD_NOREPEAT, VK_OEM_3);
        var ok2 = RegisterHotKey(messageWindow, IdPreviousTab, MOD_WIN | MOD_SHIFT | MOD_NOREPEAT, VK_OEM_3);
        registered = ok1 || ok2;
        if (!ok1 || !ok2)
            // another program (e.g. Groupy, which uses the same hotkeys) already uses the hotkey
            CrashLog.Write("Hotkeys", new InvalidOperationException(
                $"Could not register {(ok1 ? "" : "Win+` ")}{(ok2 ? "" : "Win+Shift+` ")}: already used by another program (Groupy uses the same hotkeys)"));
    }

    public static void Stop()
    {
        if (messageWindow == 0) return;
        Register(false);
        DestroyWindow(messageWindow);
        messageWindow = 0;
    }

    static nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM_HOTKEY)
        {
            try
            {
                OnHotkey((int)wParam);
            }
            catch (Exception ex)
            {
                CrashLog.Write("Hotkeys", ex);
            }
            return 0;
        }
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    static void OnHotkey(int id)
    {
        var foreground = WindowEx.ForegroundWindow;
        if (UnitedSetsApp.Current.FindSetWindowOf(foreground) is not { } set) return;
        set.SelectAdjacentTab(id is IdNextTab ? 1 : -1);
        // keep the keyboard focus in the set
        if (set.SelectedTab is { } tab)
            tab.Focus();
    }

    static readonly nint HWND_MESSAGE = -3;
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
    static extern bool DestroyWindow(nint hWnd);
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    static extern bool UnregisterHotKey(nint hWnd, int id);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
    static extern nint GetModuleHandle(string? lpModuleName);
}
