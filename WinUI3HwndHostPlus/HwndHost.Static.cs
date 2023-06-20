using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using System.Threading;
using WindowEx = WinWrapper.Windowing.Window;
using System.Linq;
using System.Drawing;
using Microsoft.UI.Xaml;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    readonly static System.Collections.Concurrent.ConcurrentDictionary<DispatcherQueue, List<HwndHost>> Dispatchers = new(5, 5);
    readonly static SynchronizedCollection<HwndHost> ActiveHwndHosts = new();
    static void AddHwndHost(HwndHost HwndHost)
    {
        var dispatcher = HwndHost.DispatcherQueue;
        if (Dispatchers.TryGetValue(dispatcher, out var list))
            list.Add(HwndHost);
        else
        {
            List<HwndHost> HwndHosts = new() { HwndHost };

            if (!Dispatchers.TryAdd(dispatcher, HwndHosts))
                if (Debugger.IsAttached)
                    Debugger.Break();

            var timer = dispatcher.CreateTimer();
            GC.KeepAlive(timer);
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += delegate
            {
                foreach (var HwndHost in HwndHosts.ToArray())
                {
                    var rect = GetBoundsRelativeToWindow(HwndHost, HwndHost.XAMLWindow, HwndHost._ParentWindow);

                    HwndHost._CacheXFromWindow = rect.X;
                    HwndHost._CacheYFromWindow = rect.Y;

                    HwndHost._CacheWidth = rect.Width;
                    HwndHost._CacheHeight = rect.Height;
                }
                timer.Start();
            };
            timer.Start();
        }
        ActiveHwndHosts.Add(HwndHost);
    }
    static RectangleF GetBoundsRelativeToWindow(UIElement Element, Window window, WindowEx windowEX)
    {
        var Pt = Element.TransformToVisual(window.Content).TransformPoint(
            new Windows.Foundation.Point(0, 0)
        );

        var scale = (float)GetScale(windowEX);
        var size = Element.ActualSize;
        return new(Pt._x * scale, Pt._y * scale, size.X * scale, size.Y * scale);
    }
    static bool IsDwmBackdropSupported = Environment.OSVersion.Version.Build > 22621;
    static void OnHwndHostLoopCalled()
    {
    Start:
        foreach (var HwndHost in ActiveHwndHosts.ToArray())
        {
            if (HwndHost.IsDisposed)
            {
                ActiveHwndHosts.Remove(HwndHost);
                goto Start;
            }
            HwndHost.ForceUpdateWindow();
        }
    }
    static HwndHost()
    {
        new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(500);
                try { OnHwndHostLoopCalled(); }
                catch { Debug.WriteLine("[HwndHostLoop] Exception Occured!"); }
            }
        })
        {
            Name = "HwndHostLoop"
        }.Start();
    }

    public static double GetScale(WindowEx Window)
        => Window.CurrentDisplay.ScaleFactor / 100.0;


    public static bool ShouldBeBlacklisted(WindowEx Window)
        => Window.Class.Name is
            "Shell_TrayWnd" // Taskbar
            or "Progman" or "WorkerW" // Desktop
            or "WindowsDashboard" // I forget
            or "Windows.UI.Core.CoreWindow" // Quick Settings and Notification Center (other uwp apps should already be ApplicationFrameHost)
        ;
}
