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

namespace UnitedSets.Classes;

public class HwndHost : FrameworkElement, IDisposable
{
    readonly MicaWindow Window;
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
    readonly DispatcherQueueTimer timer;
    readonly WINDOW_STYLE InitialStyle;
    readonly WINDOW_EX_STYLE InitialExStyle;
    public HwndHost(MicaWindow Window, WindowEx WindowToHost)
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
        timer = DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Tick += delegate
        {
            ForceUpdateWindow();
        };
        timer.Start();
        VisiblePropertyChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, Propchanged);
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
    int CountDown = 5;
    public async void ForceUpdateWindow()
    {
        var WindowToHost = this.WindowToHost;
        bool Check = false;
        if (CountDown > 0)
        {
            CountDown--;
            if (CountDown == 0) WindowToHost.Redraw();
        }
        else Check = true;
        if (XamlRoot is null) return;
        var windowpos = WinUI.Position;
        var Pt = TransformToVisual(Window.Content).TransformPoint(
            new Windows.Foundation.Point(0, 0)
        );

        var scale = GetScale(WinUIWindow);
        Pt.X = windowpos.X + Pt.X * scale;
        Pt.Y = windowpos.Y + Pt.Y * scale;
        var Size = ActualSize;
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
        if (IsWindowVisible)
        {
            var YShift = WinUIWindow.IsMaximized ? 8 : 0;
            var oldBounds = WindowToHost.Bounds;
            var newBounds = new Rectangle(
                (int)Pt._x + 8,
                (int)Pt._y + YShift,
                (int)(Size.X * scale),
                (int)(Size.Y * scale)
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
                if (new WinWrapper.WindowRelative(WindowToHost).GetAboves().Take(10).Any(x => x == WinUIWindow))
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
        WindowToHost.IsVisible = IsWindowVisible;
    }
    public static double GetScale(WindowEx Window)
        => Window.CurrentDisplay.ScaleFactor / 100.0;
    public bool IsDisposed { get; private set; }
    public void Dispose()
    {
        IsDisposed = true;
        timer.Stop();
        SizeChanged -= WinUIAppWindowChanged;
        WinUI.Changed -= WinUIAppWindowChanged;
        UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
        Closed?.Invoke();
        GC.SuppressFinalize(this);
        return;
    }
}