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

namespace UnitedSets.Classes;

public class HwndHost : FrameworkElement, IDisposable
{
    readonly MicaWindow Window;
    readonly AppWindow WinUI;
    readonly WindowEx WindowToHost;
    readonly WindowEx WinUIWindow;
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
    long VisiblePropertyChangedToken;
    DispatcherQueueTimer timer;
    WINDOW_STYLE InitialStyle;
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
        InitialStyle = WindowToHost.Style;
        //WindowToHost.Style &= ~(WindowStyles.WS_CAPTION | WindowStyles.WS_BORDER);
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

    void WinUIAppWindowChanged(AppWindow _1, AppWindowChangedEventArgs ChangedArgs)
    {
        ForceUpdateWindow();
    }
    void WinUIAppWindowChanged(object sender, SizeChangedEventArgs e)
    {
        ForceUpdateWindow();
    }
    public void DetachAndDispose()
    {
        Dispose();
        var WindowToHost = this.WindowToHost;
        WindowToHost.Style = InitialStyle;
        WindowToHost.Owner = default;
        WindowToHost.IsResizable = _DefaultIsResizable;
        WindowToHost.IsVisible = true;
    }

    public void FocusWindow() => WindowToHost.Focus();

    public event Action? Closed;
    public event Action? Updating;
    public void ForceUpdateWindow()
    {
        if (XamlRoot is null) return;
        var windowpos = WinUI.Position;
        var Pt = TransformToVisual(Window.Content).TransformPoint(
            new Windows.Foundation.Point(0, 0)
        );

        var scale = GetScale(WinUIWindow);
        Pt.X = windowpos.X + Pt.X * scale;
        Pt.Y = windowpos.Y + Pt.Y * scale;
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
            Dispose();
            return;
        }
        Updating?.Invoke();
        if (IsWindowVisible)
        {
            var YShift = WinUIWindow.IsMaximized ? 8 : 0;
            WindowToHost.Bounds = new Rectangle(
                (int)Pt._x + 8,
                (int)Pt._y + YShift,
                (int)(Size.X * scale),
                (int)(Size.Y * scale)
            );
        }
        WindowToHost.IsVisible = IsWindowVisible;
    }
    public static double GetScale(WindowEx Window)
        => Window.CurrentDisplay.ScaleFactor / 100.0;

    public void Dispose()
    {
        timer.Stop();
        SizeChanged -= WinUIAppWindowChanged;
        WinUI.Changed -= WinUIAppWindowChanged;
        UnregisterPropertyChangedCallback(VisibilityProperty, VisiblePropertyChangedToken);
        Closed?.Invoke();
        return;
    }
}