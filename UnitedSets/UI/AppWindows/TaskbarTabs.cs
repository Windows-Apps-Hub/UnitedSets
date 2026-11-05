using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using UnitedSets.Configurations;
using UnitedSets.Helpers;
using UnitedSets.Mvvm.Services;
using UnitedSets.Tabs;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// Groupy-like taskbar integration for a set window:
/// <list type="bullet">
/// <item>hovering the set's taskbar button shows one thumbnail per tab (with the app icon and title), clicking a
/// thumbnail selects that tab and its close button closes the tab (Windows "tabbed thumbnails", like old Edge);</item>
/// <item>the taskbar button uses the icon of the selected tab's application.</item>
/// </list>
/// </summary>
/// <remarks>
/// Each tab gets an invisible proxy window registered with <c>ITaskbarList3::RegisterTab</c>. The taskbar asks the proxy
/// for its thumbnail bitmap (<c>WM_DWMSENDICONICTHUMBNAIL</c>). Windows of background tabs are hidden and can't be
/// captured, so the last snapshot taken while the tab was visible is used.
/// </remarks>
sealed class TaskbarTabs : IDisposable
{
    readonly MainWindow owner;
    readonly nint ownerHwnd;
    readonly Dictionary<TabBase, Proxy> proxies = [];
    static ITaskbarList3? taskbar;
    static WndProcDelegate? wndProc;
    static readonly Dictionary<nint, Proxy> proxyByHwnd = [];
    static bool classRegistered;
    static nint unitedSetsIcon;
    bool disposed;

    sealed class Proxy
    {
        public required TabBase Tab;
        public required TaskbarTabs Owner;
        public nint Hwnd;
        public Bitmap? Snapshot;
        public nint IconSent;
        public string TitleSent = "";
    }

    static bool Enabled => UnitedSetsApp.Current.Settings.TaskbarTabThumbnails.Value;

    public TaskbarTabs(MainWindow owner)
    {
        this.owner = owner;
        ownerHwnd = owner.Win32Window.Handle;
        owner.Tabs.CollectionChanged += OnTabsChanged;
        UnitedSetsApp.Current.Settings.TaskbarTabThumbnails.PropertyChanged += (_, _) => Sync();
    }

    void OnTabsChanged(object? sender, NotifyCollectionChangedEventArgs e) => Sync();

    static ITaskbarList3? Taskbar
    {
        get
        {
            if (taskbar is not null) return taskbar;
            try
            {
                var t = (ITaskbarList3)new CTaskbarList();
                t.HrInit();
                taskbar = t;
            }
            catch (Exception ex)
            {
                CrashLog.Write("Taskbar", ex);
            }
            return taskbar;
        }
    }

    /// <summary>
    /// Creates/removes the proxies to match the tabs, their order and the selected tab. Call on the UI thread.
    /// </summary>
    public void Sync()
    {
        if (disposed) return;
        try
        {
            var tabs = Enabled ? owner.Tabs.ToArray() : [];
            foreach (var (tab, proxy) in proxies.ToArray())
                if (!tabs.Contains(tab))
                    RemoveProxy(tab, proxy);
            if (tabs.Length is 0 || Taskbar is not { } tb)
            {
                UpdateTaskbarIcon();
                return;
            }
            foreach (var tab in tabs)
                if (!proxies.ContainsKey(tab) && CreateProxy(tab) is { } proxy)
                {
                    proxies[tab] = proxy;
                    // the set's button represents the app now (windows that force a taskbar button, like
                    // WinForms ones, would show a second one)
                    foreach (var w in tab.Windows)
                        try { tb.DeleteTab(w.Handle); } catch { }
                }
            // order: insert each one before the next, from the last one
            for (int i = tabs.Length - 1; i >= 0; i--)
                if (proxies.TryGetValue(tabs[i], out var p))
                    tb.SetTabOrder(p.Hwnd, i + 1 < tabs.Length && proxies.TryGetValue(tabs[i + 1], out var next) ? next.Hwnd : 0);
            UpdateSelection();
            UpdateTitlesAndIcons();
        }
        catch (Exception ex)
        {
            CrashLog.Write("Taskbar tabs", ex);
        }
    }

    /// <summary>
    /// Call when the selected tab changes. The previous tab's window is still visible at this point:
    /// keep a picture of it for its thumbnail (it can't be captured once hidden).
    /// </summary>
    public void OnSelectionChanged(TabBase? previous)
    {
        if (disposed) return;
        if (previous is not null && proxies.TryGetValue(previous, out var p))
            TakeSnapshot(p);
        UpdateSelection();
    }

    /// <summary>
    /// Call when the selected tab changes
    /// </summary>
    public void UpdateSelection()
    {
        if (disposed) return;
        try
        {
            if (owner.SelectedTab is { } tab && proxies.TryGetValue(tab, out var p))
                Taskbar?.SetTabActive(p.Hwnd, ownerHwnd, 0);
            UpdateTaskbarIcon();
        }
        catch (Exception ex)
        {
            CrashLog.Write("Taskbar tabs", ex);
        }
    }

    /// <summary>
    /// Updates titles/icons of the thumbnails (called periodically from the UI thread).
    /// </summary>
    public void UpdateTitlesAndIcons()
    {
        if (disposed) return;
        foreach (var (tab, p) in proxies)
        {
            var title = tab.Title;
            if (title != p.TitleSent)
            {
                SetWindowText(p.Hwnd, title);
                p.TitleSent = title;
                DwmInvalidateIconicBitmaps(p.Hwnd);
            }
            var icon = GetAppIcon(tab, big: false);
            if (icon != p.IconSent)
            {
                SendMessage(p.Hwnd, WM_SETICON, ICON_SMALL, icon);
                SendMessage(p.Hwnd, WM_SETICON, ICON_BIG, GetAppIcon(tab, big: true));
                p.IconSent = icon;
            }
        }
        RefreshSelectedSnapshot();
        UpdateTaskbarIcon();
    }

    string lastIconKey = "";
    nint compositeIcon;
    /// <summary>
    /// The set's taskbar button (and Alt+Tab) shows the icons of its apps combined into one, like Groupy
    /// (a 2x2 grid for 4 tabs or more). With a single tab, that app's icon is used as is.
    /// </summary>
    void UpdateTaskbarIcon()
    {
        var icons = owner.Tabs.ToArray().Select(t => GetAppIcon(t, big: true)).Where(i => i != 0).Take(4).ToArray();
        var key = string.Join(",", icons);
        if (key == lastIconKey) return;
        lastIconKey = key;
        nint icon = icons.Length switch
        {
            0 => UnitedSetsIcon,
            1 => icons[0],
            _ => CreateCompositeIcon(icons)
        };
        SendMessage(ownerHwnd, WM_SETICON, ICON_SMALL, icon);
        SendMessage(ownerHwnd, WM_SETICON, ICON_BIG, icon);
        var old = compositeIcon;
        compositeIcon = icons.Length > 1 ? icon : 0;
        if (old != 0 && old != icon) DestroyIcon(old);
    }

    /// <summary>
    /// Draws up to 4 icons in one: side by side for 2, two on top and one below for 3, a 2x2 grid for 4.
    /// </summary>
    static nint CreateCompositeIcon(nint[] icons)
    {
        const int size = 64, cell = size / 2, pad = 1;
        using var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(System.Drawing.Color.Transparent);
            System.Drawing.Point[] cells = icons.Length switch
            {
                2 => [new(0, cell / 2), new(cell, cell / 2)],
                3 => [new(0, 0), new(cell, 0), new(cell / 2, cell)],
                _ => [new(0, 0), new(cell, 0), new(0, cell), new(cell, cell)]
            };
            for (int i = 0; i < icons.Length && i < cells.Length; i++)
            {
                try
                {
                    using var ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(icons[i]).Clone();
                    using var img = ico.ToBitmap();
                    g.DrawImage(img, new System.Drawing.Rectangle(cells[i].X + pad, cells[i].Y + pad, cell - 2 * pad, cell - 2 * pad));
                }
                catch { }
            }
        }
        return bmp.GetHicon();
    }

    static nint UnitedSetsIcon
    {
        get
        {
            if (unitedSetsIcon != 0) return unitedSetsIcon;
            var ico = UnitedSetsApp.Current.Configuration.MainConfiguration.TaskbarIco ?? "Assets/UnitedSets.ico";
            var path = Path.IsPathRooted(ico) ? ico : Path.Combine(USConfig.RootLocation, ico);
            if (!File.Exists(path)) path = Path.Combine(USConfig.RootLocation, "Assets", "UnitedSets.ico");
            unitedSetsIcon = LoadImage(0, path, 1 /* IMAGE_ICON */, 0, 0, 0x10 | 0x40 /* LR_LOADFROMFILE | LR_DEFAULTSIZE */);
            return unitedSetsIcon;
        }
    }

    static nint GetAppIcon(TabBase tab, bool big)
    {
        var window = tab.Windows.FirstOrDefault();
        if (!window.IsValid) return 0;
        var hwnd = window.Handle;
        // don't hang on unresponsive windows
        if (SendMessageTimeout(hwnd, WM_GETICON, big ? ICON_BIG : ICON_SMALL2, 0, SMTO_ABORTIFHUNG, 100, out var icon) != 0 && icon != 0)
            return icon;
        if (big && SendMessageTimeout(hwnd, WM_GETICON, ICON_SMALL2, 0, SMTO_ABORTIFHUNG, 100, out icon) != 0 && icon != 0)
            return icon;
        icon = GetClassLongPtr(hwnd, big ? GCLP_HICON : GCLP_HICONSM);
        if (icon == 0) icon = GetClassLongPtr(hwnd, GCLP_HICON);
        return icon;
    }

    Proxy? CreateProxy(TabBase tab)
    {
        EnsureClass();
        var hwnd = CreateWindowEx(WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW, ProxyClass, tab.Title,
            WS_POPUP | WS_BORDER | WS_SYSMENU | WS_CAPTION, -32000, -32000, 10, 10, 0, 0, GetModuleHandle(null), 0);
        if (hwnd == 0) return null;
        int yes = 1;
        DwmSetWindowAttribute(hwnd, DWMWA_FORCE_ICONIC_REPRESENTATION, ref yes, sizeof(int));
        DwmSetWindowAttribute(hwnd, DWMWA_HAS_ICONIC_BITMAP, ref yes, sizeof(int));
        var proxy = new Proxy { Tab = tab, Owner = this, Hwnd = hwnd, TitleSent = tab.Title };
        proxyByHwnd[hwnd] = proxy;
        UnitedSetsApp.Current.RegisterUnitedSetsWindow(WindowEx.FromWindowHandle(hwnd));
        Taskbar?.RegisterTab(hwnd, ownerHwnd);
        return proxy;
    }

    void RemoveProxy(TabBase tab, Proxy proxy)
    {
        proxies.Remove(tab);
        proxyByHwnd.Remove(proxy.Hwnd);
        try { Taskbar?.UnregisterTab(proxy.Hwnd); } catch { }
        UnitedSetsApp.Current.UnregisterUnitedSetsWindow(WindowEx.FromWindowHandle(proxy.Hwnd));
        DestroyWindow(proxy.Hwnd);
        proxy.Snapshot?.Dispose();
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        owner.Tabs.CollectionChanged -= OnTabsChanged;
        if (compositeIcon != 0) DestroyIcon(compositeIcon);
        foreach (var (tab, proxy) in proxies.ToArray())
            RemoveProxy(tab, proxy);
    }

    // ---- snapshots ----

    /// <summary>
    /// Takes a picture of the tab's window, only if it is visible on screen and nothing covers it.
    /// </summary>
    /// <remarks>
    /// The pixels are copied from the screen. <c>PrintWindow</c> was used before: it makes the application redraw
    /// its whole window, which made windows like Chrome flicker (as if resized) every time Windows asked for a
    /// thumbnail, e.g. when switching to another app with Alt+Tab or the taskbar.
    /// </remarks>
    static void TakeSnapshot(Proxy p)
    {
        var window = p.Tab.Windows.FirstOrDefault();
        if (!window.IsValid || !window.IsVisible) return;
        var bmp = CaptureFromScreen(window.Handle);
        if (bmp is null) return;
        var old = p.Snapshot;
        p.Snapshot = bmp;
        old?.Dispose();
    }

    static Bitmap? CaptureFromScreen(nint hwnd)
    {
        if (IsIconic(hwnd)) return null;
        if (DwmGetWindowAttribute(hwnd, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT r, Marshal.SizeOf<RECT>()) != 0 &&
            !GetWindowRect(hwnd, out r))
            return null;
        int w = r.Right - r.Left, h = r.Bottom - r.Top;
        if (w <= 8 || h <= 8 || w > 16384 || h > 16384) return null;
        // nothing (another window, Alt+Tab, a menu...) may cover it, or we would capture that instead
        for (int ix = 1; ix <= 3; ix++)
            for (int iy = 1; iy <= 3; iy++)
            {
                var pt = new POINT { X = r.Left + w * ix / 4, Y = r.Top + h * iy / 4 };
                if (GetAncestor(WindowFromPoint(pt), GA_ROOT) != hwnd)
                    return null;
            }
        var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
            g.CopyFromScreen(r.Left, r.Top, 0, 0, new System.Drawing.Size(w, h));
        return bmp;
    }

    /// <summary>
    /// Refreshes the picture of the selected tab while the set is in use (called periodically)
    /// </summary>
    void RefreshSelectedSnapshot()
    {
        if (owner.SelectedTab is not { } tab || !proxies.TryGetValue(tab, out var p)) return;
        var fg = GetForegroundWindow();
        if (fg != ownerHwnd && !tab.ContainsWindow(fg)) return;
        TakeSnapshot(p);
    }

    static Bitmap MakeThumbnail(Proxy p, int maxWidth, int maxHeight)
    {
        var source = p.Snapshot;
        int w = maxWidth, h = maxHeight;
        if (source is not null)
        {
            var scale = Math.Min((double)maxWidth / source.Width, (double)maxHeight / source.Height);
            w = Math.Max(1, (int)(source.Width * scale));
            h = Math.Max(1, (int)(source.Height * scale));
        }
        var bmp = new Bitmap(w, h, PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(bmp);
        if (source is not null)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(source, 0, 0, w, h);
        }
        else
        {
            // never shown yet: the app icon on a neutral background
            g.Clear(System.Drawing.Color.FromArgb(255, 243, 243, 243));
            var icon = GetAppIcon(p.Tab, big: true);
            if (icon != 0)
            {
                var size = Math.Min(48, Math.Min(w, h) / 2);
                var hdc = g.GetHdc();
                try { DrawIconEx(hdc, (w - size) / 2, (h - size) / 2, icon, size, size, 0, 0, 3 /* DI_NORMAL */); }
                finally { g.ReleaseHdc(hdc); }
            }
        }
        return bmp;
    }

    /// <summary>
    /// DWM only accepts 32 bit (premultiplied alpha) DIB sections
    /// </summary>
    static unsafe nint CreateDib(Bitmap source, bool topDown)
    {
        int w = source.Width, h = source.Height;
        var info = new BITMAPINFOHEADER
        {
            biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
            biWidth = w,
            biHeight = topDown ? -h : h,
            biPlanes = 1,
            biBitCount = 32
        };
        var hbmp = CreateDIBSection(0, ref info, 0 /* DIB_RGB_COLORS */, out var bits, 0, 0);
        if (hbmp == 0) return 0;
        var data = source.LockBits(new System.Drawing.Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
        try
        {
            for (int y = 0; y < h; y++)
            {
                var row = topDown ? y : h - 1 - y;
                Buffer.MemoryCopy((byte*)data.Scan0 + y * data.Stride, (byte*)bits + row * w * 4, w * 4, w * 4);
            }
        }
        finally
        {
            source.UnlockBits(data);
        }
        return hbmp;
    }

    // ---- proxy window procedure ----

    const string ProxyClass = "UnitedSetsTaskbarTab";
    static void EnsureClass()
    {
        if (classRegistered) return;
        wndProc = ProxyWndProc;
        var wc = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc),
            hInstance = GetModuleHandle(null),
            lpszClassName = ProxyClass
        };
        RegisterClassEx(ref wc);
        classRegistered = true;
    }

    static nint ProxyWndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        try
        {
            if (!proxyByHwnd.TryGetValue(hwnd, out var p))
                return DefWindowProc(hwnd, msg, wParam, lParam);
            switch (msg)
            {
                case WM_ACTIVATE when (wParam & 0xFFFF) != 0 /* not WA_INACTIVE */:
                    // the user clicked the thumbnail
                    p.Owner.owner.SelectedTab = p.Tab;
                    p.Owner.owner.Activate();
                    return 0;
                case WM_SYSCOMMAND when (wParam & 0xFFF0) == SC_CLOSE:
                case WM_CLOSE:
                    // the close button of the thumbnail: same as closing the tab
                    p.Tab.TabCloseClickEv(null!, null!);
                    return 0;
                case WM_DWMSENDICONICTHUMBNAIL:
                {
                    // (measured: the width is in the low word and the height in the high word)
                    int maxWidth = (int)(lParam & 0xFFFF), maxHeight = (int)((lParam >> 16) & 0xFFFF);
                    // no capture here: use the last picture (capturing now would capture Alt+Tab/the taskbar)
                    using var thumb = MakeThumbnail(p, maxWidth, maxHeight);
                    var hbmp = CreateDib(thumb, topDown: false);
                    var hr = DwmSetIconicThumbnail(hwnd, hbmp, 0);
                    DeleteObject(hbmp);
                    if (hr < 0) CrashLog.Write("Taskbar thumbnail", Marshal.GetExceptionForHR(hr));
                    return 0;
                }
                case WM_DWMSENDICONICLIVEPREVIEWBITMAP:
                {
                    // peek: the set with this tab's window in place (last picture)
                    var window = p.Tab.Windows.FirstOrDefault();
                    if (p.Snapshot is { } snap && window.IsValid &&
                        DwmGetWindowAttribute(window.Handle, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT wr, Marshal.SizeOf<RECT>()) == 0 &&
                        GetWindowRect(p.Owner.ownerHwnd, out var sr))
                    {
                        var hbmp = CreateDib(snap, topDown: false);
                        var offset = new POINT { X = wr.Left - sr.Left, Y = wr.Top - sr.Top };
                        var hr = DwmSetIconicLivePreviewBitmap(hwnd, hbmp, ref offset, 0);
                        DeleteObject(hbmp);
                        if (hr < 0) CrashLog.Write("Taskbar live preview", Marshal.GetExceptionForHR(hr));
                    }
                    return 0;
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.Write("Taskbar tab proxy", ex);
        }
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    // ---- interop ----

    const uint WM_ACTIVATE = 0x0006, WM_CLOSE = 0x0010, WM_SYSCOMMAND = 0x0112, WM_SETICON = 0x0080, WM_GETICON = 0x007F;
    const uint WM_DWMSENDICONICTHUMBNAIL = 0x0323, WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326;
    const int SC_CLOSE = 0xF060;
    const nint ICON_SMALL = 0, ICON_BIG = 1, ICON_SMALL2 = 2;
    const int GCLP_HICON = -14, GCLP_HICONSM = -34;
    const uint SMTO_ABORTIFHUNG = 0x2;
    const uint WS_POPUP = 0x80000000, WS_BORDER = 0x00800000, WS_SYSMENU = 0x00080000, WS_CAPTION = 0x00C00000;
    const uint WS_EX_NOACTIVATE = 0x08000000, WS_EX_TOOLWINDOW = 0x80;
    const int DWMWA_FORCE_ICONIC_REPRESENTATION = 7, DWMWA_HAS_ICONIC_BITMAP = 10;
    const uint GA_ROOT = 2;
    const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

    [ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090"), ClassInterface(ClassInterfaceType.None)]
    class CTaskbarList { }

    [ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITaskbarList3
    {
        // ITaskbarList
        void HrInit();
        void AddTab(nint hwnd);
        void DeleteTab(nint hwnd);
        void ActivateTab(nint hwnd);
        void SetActiveAlt(nint hwnd);
        // ITaskbarList2
        void MarkFullscreenWindow(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool fullscreen);
        // ITaskbarList3
        void SetProgressValue(nint hwnd, ulong completed, ulong total);
        void SetProgressState(nint hwnd, int flags);
        void RegisterTab(nint hwndTab, nint hwndMDI);
        void UnregisterTab(nint hwndTab);
        void SetTabOrder(nint hwndTab, nint hwndInsertBefore);
        void SetTabActive(nint hwndTab, nint hwndMDI, uint reserved);
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
    [StructLayout(LayoutKind.Sequential)]
    struct RECT { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)]
    struct POINT { public int X, Y; }
    [StructLayout(LayoutKind.Sequential)]
    struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth, biHeight;
        public ushort biPlanes, biBitCount;
        public uint biCompression, biSizeImage;
        public int biXPelsPerMeter, biYPelsPerMeter;
        public uint biClrUsed, biClrImportant;
    }
    [DllImport("gdi32.dll")]
    static extern nint CreateDIBSection(nint hdc, ref BITMAPINFOHEADER info, uint usage, out nint bits, nint section, uint offset);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "RegisterClassExW")]
    static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateWindowExW")]
    static extern nint CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam);
    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);
    [DllImport("user32.dll")]
    static extern bool DestroyWindow(nint hwnd);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowTextW")]
    static extern bool SetWindowText(nint hwnd, string text);
    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    static extern nint SendMessage(nint hwnd, uint msg, nint wParam, nint lParam);
    [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW")]
    static extern nint SendMessageTimeout(nint hwnd, uint msg, nint wParam, nint lParam, uint flags, uint timeout, out nint result);
    [DllImport("user32.dll", EntryPoint = "GetClassLongPtrW")]
    static extern nint GetClassLongPtr(nint hwnd, int index);
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(nint hwnd, out RECT rect);
    [DllImport("user32.dll")]
    static extern nint WindowFromPoint(POINT pt);
    [DllImport("user32.dll")]
    static extern nint GetAncestor(nint hwnd, uint flags);
    [DllImport("user32.dll")]
    static extern nint GetForegroundWindow();
    [DllImport("user32.dll")]
    static extern bool IsIconic(nint hwnd);
    [DllImport("user32.dll")]
    static extern bool DestroyIcon(nint icon);
    [DllImport("dwmapi.dll")]
    static extern int DwmGetWindowAttribute(nint hwnd, int attribute, out RECT value, int size);
    [DllImport("user32.dll")]
    static extern bool DrawIconEx(nint hdc, int x, int y, nint icon, int cx, int cy, uint step, nint brush, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadImageW")]
    static extern nint LoadImage(nint hInst, string name, uint type, int cx, int cy, uint fuLoad);
    [DllImport("gdi32.dll")]
    static extern bool DeleteObject(nint obj);
    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int value, int size);
    [DllImport("dwmapi.dll")]
    static extern int DwmSetIconicThumbnail(nint hwnd, nint hbmp, uint flags);
    [DllImport("dwmapi.dll")]
    static extern int DwmSetIconicLivePreviewBitmap(nint hwnd, nint hbmp, ref POINT client, uint flags);
    [DllImport("dwmapi.dll")]
    static extern int DwmInvalidateIconicBitmaps(nint hwnd);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
    static extern nint GetModuleHandle(string? lpModuleName);
}
