using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using WinWrapper;
using Windows.Win32.UI.WindowsAndMessaging;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace UnitedSets.Classes;

public class HwndHost : FrameworkElement, IDisposable
{
    static System.Collections.Concurrent.ConcurrentDictionary<DispatcherQueue, List<HwndHost>> Dispatchers = new(5, 5);
    static SynchronizedCollection<HwndHost> ActiveHwndHosts = new();
    static void AddHwndHost(HwndHost HwndHost)
    {
        var dispatcher = HwndHost.DispatcherQueue;
        if (Dispatchers.TryGetValue(dispatcher, out var list))
        {
            list.Add(HwndHost);
        }
        else
        {
            List<HwndHost> HwndHosts = new()
            {
                HwndHost
            };
            if (!Dispatchers.TryAdd(dispatcher, HwndHosts))
                if (Debugger.IsAttached)
                    Debugger.Break();
            var timer = dispatcher.CreateTimer();
            GC.KeepAlive(timer);
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += delegate
            {
                foreach (var HwndHost in HwndHosts)
                {
                    var Pt = HwndHost.TransformToVisual(HwndHost.Window.Content).TransformPoint(
                        new Windows.Foundation.Point(0, 0)
                    );
                    var size = HwndHost.ActualSize;

                    HwndHost.CacheXFromWindow = Pt._x;
                    HwndHost.CacheYFromWindow = Pt._y;

                    HwndHost.CacheWidth = size.X;
                    HwndHost.CacheHeight = size.Y;
                }
                timer.Start();
            };
            timer.Start();
        }
        ActiveHwndHosts.Add(HwndHost);
    }
    public float CacheXFromWindow { get; private set; }
    public float CacheYFromWindow { get; private set; }
    public float CacheWidth { get; private set; }
    public float CacheHeight { get; private set; }
    static HwndHost()
    {
        new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                Start:
                    foreach (var HwndHost in ActiveHwndHosts)
                    {
                        if (HwndHost.IsDisposed)
                        {
                            ActiveHwndHosts.Remove(HwndHost);
                            goto Start;
                        }
                        HwndHost.ForceUpdateWindow();
                    }
                }
                catch
                {
                    Debug.WriteLine("[HwndHostLoop] Exception Occured!");
                }
            }
        })
        {
            Name = "HwndHostLoop"
        }.Start();
    }

    readonly MainWindow Window;
    readonly AppWindow WinUI;
    readonly WindowEx WindowToHost;
    readonly WindowEx WinUIWindow;
    bool IsOwnerSetSuccessful;
    bool _IsWindowVisible;
    bool _DefaultIsResizable;
    public bool IsWindowVisible
    {
        get => _IsWindowVisible;
        set
        {
            _IsWindowVisible = value;
            ForceUpdateWindow();
        }
    }
    readonly long VisiblePropertyChangedToken;
    readonly WINDOW_STYLE InitialStyle;
    //readonly WINDOW_EX_STYLE InitialExStyle;
    public HwndHost(MainWindow Window, WindowEx WindowToHost)
    {
        this.Window = Window;
        var WinUIHandle = WinRT.Interop.WindowNative.GetWindowHandle(Window);
        WinUI = AppWindow.GetFromWindowId(
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                WinUIHandle
            )
        );
        this.WindowToHost = WindowToHost;
        _DefaultIsResizable = WindowToHost.IsResizable;
        WinUIWindow = WindowEx.FromWindowHandle(WinUIHandle);
        var bound = WindowToHost.Bounds;
        WindowToHost.Owner = WinUIWindow;
        IsOwnerSetSuccessful = WindowToHost.Owner == WinUIWindow;
        InitialStyle = WindowToHost.Style;
        //InitialExStyle = WindowToHost.ExStyle;
        //if (!IsOwnerSetSuccessful) WindowToHost.ExStyle |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        WinUI.Changed += WinUIAppWindowChanged;
        SizeChanged += WinUIAppWindowChanged;

        VisiblePropertyChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, Propchanged);
        AddHwndHost(this);
    }
    void Propchanged(DependencyObject _, DependencyProperty _1) => ForceUpdateWindow();
    void WinUIAppWindowChanged(AppWindow _1, AppWindowChangedEventArgs ChangedArgs) => ForceUpdateWindow();
    void WinUIAppWindowChanged(object sender, SizeChangedEventArgs e) => ForceUpdateWindow();
    public void DetachAndDispose()
    {
        Dispose();
        var WindowToHost = this.WindowToHost;
        WindowToHost.Style = InitialStyle;
        //WindowToHost.ExStyle = InitialExStyle;
        WindowToHost.Owner = default;
        WindowToHost.IsResizable = _DefaultIsResizable;
        WindowToHost.IsVisible = true;
    }

    public void FocusWindow() => WindowToHost.Focus();

    public event Action? Closed;
    public event Action? Updating;
    //public event Action? Detaching;
    int CountDown = 5;
    public async void ForceUpdateWindow()
    {
        if (CacheWidth == 0 || CacheHeight == 0) return; // wait for update
        if (IsDisposed) return;

        var WindowToHost = this.WindowToHost;
        WindowToHost.IsVisible = IsWindowVisible;
        if (!IsWindowVisible) return;
        
        bool Check = false;
        if (CountDown > 0)
        {
            CountDown--;
            if (CountDown == 0) WindowToHost.Redraw();
        }
        else Check = true;
        var windowbounds = WinUIWindow.Bounds;

        var scale = GetScale(WinUIWindow);
        var Pt = new Point
        {
            X = (int)(windowbounds.X + CacheXFromWindow * scale),
            Y = (int)(windowbounds.Y + CacheYFromWindow * scale)
        };
        try
        {
            WindowToHost.IsResizable = false;
        }
        catch
        {

        }
        if (!WindowToHost.IsValid)
        {
            Dispose();
            return;
        }
        Updating?.Invoke();
        var YShift = WinUIWindow.IsMaximized ? 8 : 0;
        var oldBounds = WindowToHost.Bounds;
        var newBounds = new Rectangle(
            Pt.X + 8,
            Pt.Y + YShift,
            (int)(CacheWidth * scale),
            (int)(CacheHeight * scale)
        );
        if (oldBounds != newBounds)
        {
            if (Check && WindowEx.ForegroundWindow == WindowToHost)
            {
                DetachAndDispose();
                return;
            }
            else WindowToHost.Bounds = newBounds;
        }
        if (!IsOwnerSetSuccessful)
        {
            if (new WindowRelative(WindowToHost).GetAboves().Take(10).Any(x => x == WinUIWindow))
            {
                await Task.Delay(500);
                if (oldBounds == WindowToHost.Bounds && IsWindowVisible)
                {
                    WindowToHost.IsVisible = false;
                    WindowToHost.IsVisible = true;
                    WindowToHost.Focus();
                }
            }
        }
    }
    public static double GetScale(WindowEx Window)
        => Window.CurrentDisplay.ScaleFactor / 100.0;
    public bool IsDisposed { get; private set; }
    public void Dispose()
    {
        DispatcherQueue.TryEnqueue(delegate
        {
            IsDisposed = true;
            SizeChanged -= WinUIAppWindowChanged;
            WinUI.Changed -= WinUIAppWindowChanged;
            UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
            Closed?.Invoke();
            GC.SuppressFinalize(this);
        });
        return;
    }
}