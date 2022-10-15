using Microsoft.UI.Xaml;
using System;
using PInvoke;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using WindowStyles = PInvoke.User32.WindowStyles;
using WindowStylesEx = PInvoke.User32.WindowStylesEx;
using System.Reflection.Metadata;

namespace UnitedSets;

public class HwndHost : FrameworkElement
{
    readonly Window Window;
    readonly AppWindow WinUI;
    readonly WindowEx IntermedieteWindow;
    readonly WindowEx WindowToHost;
    readonly WindowEx WinUIWindow;
    bool _IsWindowVisible;
    public bool IsWindowVisible {
        get => _IsWindowVisible;
        set {
            _IsWindowVisible = value;
            ForceUpdateWindow();
        }
    }
    public HwndHost(Window Window, WindowEx WindowToHost)
    {
        this.Window = Window;
        var WinUIHandle = WinRT.Interop.WindowNative.GetWindowHandle(Window);
        WinUI = AppWindow.GetFromWindowId(
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                WinUIHandle
            )
        );
        this.WindowToHost = WindowToHost;
        WinUIWindow = WindowEx.FromWindowHandle(WinUIHandle);
        var bound = WindowToHost.Bounds;
        IntermedieteWindow = WindowEx.CreateNewWindow("", new Rectangle
        {
            X = 0,
            Y = 0,
            Width = bound.Width,
            Height = bound.Height
        });
        //IntermedieteWindow.Owner = WinUIWindow;
        //IntermedieteWindow.Style &= ~(WindowStyles.WS_CAPTION | WindowStyles.WS_BORDER);
        //IntermedieteWindow.ExStyle =
        //    WindowStylesEx.WS_EX_TOOLWINDOW |
        //    WindowStylesEx.WS_EX_TRANSPARENT;
        WindowToHost.Owner = WinUIWindow;
        WindowToHost.Style &= ~(WindowStyles.WS_CAPTION | WindowStyles.WS_BORDER);
        WinUI.Changed += WinUIAppWindowChanged;
        SizeChanged += WinUIAppWindowChanged;
        RegisterPropertyChangedCallback(VisibilityProperty, (_, _) => ForceUpdateWindow());
    }
    protected override void OnDisconnectVisualChildren()
    {
        //var WindowToHost = this.WindowToHost;
        //WindowToHost.IsVisible = false;
    }

    void WinUIAppWindowChanged(AppWindow _1, AppWindowChangedEventArgs ChangedArgs)
    {
        ForceUpdateWindow();
    }
    void WinUIAppWindowChanged(object sender, SizeChangedEventArgs e)
    {
        ForceUpdateWindow();
    }
    public void FocusWindow()
    {
        WindowToHost.Focus();
    }
    public event Action? Closed;
    public void ForceUpdateWindow()
    {
        if (XamlRoot is null) return;
        var windowpos = WinUI.Position;
        var Pt = TransformToVisual(Window.Content).TransformPoint(
            new Windows.Foundation.Point(windowpos.X, windowpos.Y)
        );
        var Size = ActualSize;
        var WindowToHost = this.WindowToHost;
        try
        {
            WindowToHost.IsResizable = false;
        }
        catch
        {

        }
        if (!WindowToHost.IsValid)
        {
            WinUI.Changed -= WinUIAppWindowChanged;
            Closed?.Invoke();
            return;
        }
        var IntermedieteWindow = this.IntermedieteWindow;
        WindowToHost.IsMinimized = IsWindowVisible; // IntermedieteWindow.IsVisible = Visibility == Visibility.Visible
        WindowToHost.IsTtileBarVisible = false;
        //IntermedieteWindow.Bounds = new Rectangle(
        //    (int)Pt._x + 5,
        //    (int)Pt._y + 2,
        //    (int)Size.X + 5,
        //    (int)Size.Y
        //);
        var YShift = WinUIWindow.IsMaximized ? 8 : 0;
         int TabMargin = 54;
        WindowToHost.Bounds = new Rectangle(
            (int)Pt._x + 8,
            (int)Pt._y + YShift + TabMargin,
            (int)(Size.X * GetScale()),
            (int)(Size.Y * GetScale())
        ); ;
    }
    public double GetScale()
    {
        var progmanWindow = NativeMethods.FindWindow("Shell_TrayWnd", null);
        var monitor = NativeMethods.MonitorFromWindow(progmanWindow, NativeMethods.MONITOR_DEFAULTTOPRIMARY);

        NativeMethods.DeviceScaleFactor scale;
        NativeMethods.GetScaleFactorForMonitor(monitor, out scale);

        if (scale == NativeMethods.DeviceScaleFactor.DEVICE_SCALE_FACTOR_INVALID)
        {
            scale = NativeMethods.DeviceScaleFactor.SCALE_100_PERCENT;
        }

        return Convert.ToDouble(scale) / 100;
    }
}

public static class NativeMethods
{
    public const int MONITOR_DEFAULTTOPRIMARY = 1;
    public const int MONITOR_DEFAULTTONEAREST = 2;

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("shcore.dll")]
    public static extern IntPtr GetScaleFactorForMonitor(IntPtr hwnd, out DeviceScaleFactor dwFlags);

    public enum DeviceScaleFactor
    {
        DEVICE_SCALE_FACTOR_INVALID = 0,
        SCALE_100_PERCENT = 100,
        SCALE_120_PERCENT = 120,
        SCALE_125_PERCENT = 125,
        SCALE_140_PERCENT = 140,
        SCALE_150_PERCENT = 150,
        SCALE_160_PERCENT = 160,
        SCALE_175_PERCENT = 175,
        SCALE_180_PERCENT = 180,
        SCALE_200_PERCENT = 200,
        SCALE_225_PERCENT = 225,
        SCALE_250_PERCENT = 250,
        SCALE_300_PERCENT = 300,
        SCALE_350_PERCENT = 350,
        SCALE_400_PERCENT = 400,
        SCALE_450_PERCENT = 450,
        SCALE_500_PERCENT = 500,
    }
}
public struct WindowEx
{
    private WindowEx(IntPtr Handle)
    {
        this.Handle = Handle;
    }
    bool HookInitialized = false;
    Action? _Changed = null;
    public event Action? Changed
    {
        add
        {
            _Changed += value;
            if (HookInitialized) return;
            //InitializeHook();
        }
        remove
        {
            _Changed -= value;
        }
    }
    public IntPtr Handle { get; }
    public override string ToString()
    {
        return IsValid ? $"Window {Handle} ({Text})" : $"Invalid Window ({Handle})";
    }
    public bool IsValid => User32.IsWindow(Handle);
    public List<WindowEx> Children
        => GetWindowAPI.EnumChildWindows(Handle);
    public WindowEx Owner
    {
        get => new(User32.GetWindow(Handle, User32.GetWindowCommands.GW_OWNER));
        set
        {
            User32.SetWindowLongPtr(Handle, User32.WindowLongIndexFlags.GWLP_HWNDPARENT, value.Handle);
        }
    }
    public int GetWindowLong(User32.WindowLongIndexFlags flag)
    {
        return User32.GetWindowLong(Handle, flag);
    }
    public IntPtr GetWindowLongIntPtr(User32.WindowLongIndexFlags flag)
    {
        return User32.GetWindowLongPtr_IntPtr(Handle, flag);
    }
    public int SetWindowLong(User32.WindowLongIndexFlags index, User32.SetWindowLongFlags setflag)
    {
        return User32.SetWindowLong(Handle, index, setflag);
    }
    public IntPtr SetWindowLong(User32.WindowLongIndexFlags index, IntPtr setflag)
    {
        return User32.SetWindowLongPtr(Handle, index, setflag);
    }
    public User32.WindowStyles Style
    {
        get => (User32.WindowStyles)GetWindowLong(User32.WindowLongIndexFlags.GWL_STYLE);
        set => User32.SetWindowLongPtr(Handle, User32.WindowLongIndexFlags.GWL_STYLE, (IntPtr)(int)value);
    }
    public User32.WindowStylesEx ExStyle
    {
        get => (User32.WindowStylesEx)GetWindowLong(User32.WindowLongIndexFlags.GWL_EXSTYLE);
        set => User32.SetWindowLongPtr(Handle, User32.WindowLongIndexFlags.GWL_EXSTYLE, (IntPtr)(int)value);
    }
    public bool IsResizable
    {
        get => (Style & User32.WindowStyles.WS_SIZEFRAME) != 0;
        set
        {
            if (value)
                Style |= User32.WindowStyles.WS_SIZEFRAME;
            else
                Style &= ~User32.WindowStyles.WS_SIZEFRAME;
        }
    }
    public bool IsVisible
    {
        get => (Style & User32.WindowStyles.WS_VISIBLE) != 0;
        set
        {
            if (value)
                Style |= User32.WindowStyles.WS_VISIBLE;
            else
                Style &= ~User32.WindowStyles.WS_VISIBLE;
        }
    }
    public bool IsTtileBarVisible
    {
        get => (Style & User32.WindowStyles.WS_CAPTION) != 0;
        set
        {
            if (value)
                Style |= User32.WindowStyles.WS_CAPTION;
            else
                Style &= ~User32.WindowStyles.WS_CAPTION;
        }
    }
    public WindowEx Parent
    {
        get => new(User32.GetAncestor(Handle, User32.GetAncestorFlags.GA_PARENT));
        set => User32.SetParent(Handle, value.Handle);
    }
    public WindowEx Root => new(User32.GetAncestor(Handle, User32.GetAncestorFlags.GA_ROOT));
    public WindowEx Above => new(User32.GetWindow(Handle, User32.GetWindowCommands.GW_HWNDPREV));
    public WindowEx Below => new(User32.GetWindow(Handle, User32.GetWindowCommands.GW_HWNDNEXT));
    public Rectangle Bounds
    {
        get
        {
            User32.GetWindowRect(Handle, out var rect);
            return new Rectangle
            {
                X = rect.left,
                Y = rect.top,
                Width = rect.right - rect.left,
                Height = rect.bottom - rect.top
            };
        }
        set
        {
            User32.SetWindowPos(Handle, IntPtr.Zero, value.X, value.Y, value.Width, value.Height,
                User32.SetWindowPosFlags.SWP_NOZORDER | User32.SetWindowPosFlags.SWP_NOACTIVATE);
        }
    }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Icon? SmallIcon
    {
        get
        {
            try
            {
                return Icon.FromHandle(User32.DefWindowProc(Handle, User32.WindowMessage.WM_GETICON, (IntPtr)2, IntPtr.Zero));
            }
            catch
            {
                return null;
            }
        }
    }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Icon? LargeIcon
    {
        get
        {
            try
            {
                return Icon.FromHandle(User32.DefWindowProc(Handle, User32.WindowMessage.WM_GETICON, (IntPtr)0, IntPtr.Zero));
            }
            catch
            {
                return null;
            }

        }
    }
    int TextLength => User32.GetWindowTextLength(Handle);
    public bool IsMaximized
    {
        get
        {
            var placement = User32.GetWindowPlacement(Handle);
            return placement.showCmd is User32.WindowShowStyle.SW_MAXIMIZE;
        }
    }
    public bool IsMinimized
    {
        get
        {
            var placement = User32.GetWindowPlacement(Handle);
            return placement.showCmd is User32.WindowShowStyle.SW_SHOWMINIMIZED;
        }
        set
        {
            if (IsMinimized == value) return;
            User32.ShowWindow(Handle, User32.WindowShowStyle.SW_RESTORE);
        }
    }
    public string Text
    {
        get
        {
            Span<char> chars = stackalloc char[TextLength + 1];
            User32.GetWindowText(Handle, chars);
            return new string(chars[..^1]);
        }
    }
    static IEnumerable<WindowEx> GetBelows(WindowEx refernece)
    {
        while (true)
        {
            refernece = refernece.Below;
            if (refernece.IsValid)
                yield return refernece;
            else
                yield break;
        }
    }
    public IEnumerable<WindowEx> GetBelows() => GetBelows(this);
    static IEnumerable<WindowEx> GetAboves(WindowEx refernece)
    {
        while (true)
        {
            refernece = refernece.Above;
            if (refernece.IsValid)
                yield return refernece;
            else
                yield break;
        }
    }
    public IEnumerable<WindowEx> GetAboves() => GetAboves(this);
    public static WindowEx FromLocation(int X, int Y) => FromLocation(new Point(X, Y));
    public static WindowEx FromLocation(Point pt)
        => new(User32.WindowFromPoint(new POINT
        {
            x = pt.X,
            y = pt.Y
        }));
    public static WindowEx CreateNewWindow(string Title, Rectangle Bounds = default)
    {
        if (Bounds == default)
        {
            const int d = User32.CW_USEDEFAULT;
            Bounds = new Rectangle(d, d, d, d);
        }
        User32.WNDCLASS cls;
        var hInstance = Kernel32.GetCurrentProcess().DangerousGetHandle();
        unsafe
        {
            fixed (char* p = "ME")
            {
                cls = new User32.WNDCLASS()
                {
                    lpszClassName = p,
                    hInstance = hInstance,
                    lpfnWndProc = (hwnd, msg, wParam, lParam) =>
                    {
                        return User32.DefWindowProc(hwnd, msg, (IntPtr)wParam, (IntPtr)lParam);
                    }
                };
                User32.RegisterClass(ref cls);
            }
        }
        return new(User32.CreateWindow(
            "ME",
            Title,
            User32.WindowStyles.WS_OVERLAPPEDWINDOW,
            Bounds.X,
            Bounds.Y,
            Bounds.Width,
            Bounds.Height,
            IntPtr.Zero,
            IntPtr.Zero,
            hInstance,
            IntPtr.Zero
        ));
    }
    public static List<WindowEx> GetAllWindows()
        => GetWindowAPI.EnumWindows();
    public static List<WindowEx> GetWindowInCurrentThread()
        => GetWindowAPI.EnumCurrentThreadWindows();
    public static List<WindowEx> GetWindowInThread(IntPtr Thread)
        => GetWindowAPI.EnumCurrentThreadWindows();
    public static List<WindowEx> GetSameThreadWindows(WindowEx Window)
        => GetWindowAPI.EnumSameThreadWindows(Window);

    public static WindowEx GetWindowFromPoint(Point pt)
        => new(User32.WindowFromPoint(new POINT { x = pt.X, y = pt.Y }));

    public static WindowEx FromWindowHandle(IntPtr Handle)
        => new(Handle);
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetFocus(IntPtr hWnd);
    public void Focus()
    {
        SetFocus(Handle);
    }
    static class GetWindowAPI
    {
        private delegate bool CallBackPtr(int hwnd, int lParam);
        private readonly static CallBackPtr callBackPtr = Callback;
        private static List<WindowEx> _WinStructList = new();

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hWndParent, CallBackPtr lpEnumFunc, IntPtr lParam);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumThreadWindows(IntPtr dwThreadId, CallBackPtr lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private static bool Callback(int hWnd, int lparam)
        {
            _WinStructList.Add(WindowEx.FromWindowHandle((IntPtr)hWnd));
            return true;
        }

        public static List<WindowEx> EnumWindows()
        {
            _WinStructList = new List<WindowEx>();
            EnumWindows(callBackPtr, IntPtr.Zero);
            return _WinStructList;
        }
        public static List<WindowEx> EnumChildWindows(IntPtr hWndParent)
        {
            _WinStructList = new List<WindowEx>();
            EnumChildWindows(hWndParent, callBackPtr, IntPtr.Zero);
            return _WinStructList;
        }
        public static List<WindowEx> EnumCurrentThreadWindows()
        {
            _WinStructList = new List<WindowEx>();
            EnumThreadWindows(Kernel32.GetCurrentThread().DangerousGetHandle(), callBackPtr, IntPtr.Zero);
            return _WinStructList;
        }
        public static List<WindowEx> EnumThreadWindows(IntPtr ThreadId)
        {
            _WinStructList = new List<WindowEx>();
            EnumThreadWindows(ThreadId, callBackPtr, IntPtr.Zero);
            return _WinStructList;
        }
        public static List<WindowEx> EnumSameThreadWindows(WindowEx Window)
        {
            _WinStructList = new List<WindowEx>();
            EnumThreadWindows((IntPtr)User32.GetWindowThreadProcessId(Window.Handle, out _), callBackPtr, IntPtr.Zero);
            return _WinStructList;
        }
    }
}
