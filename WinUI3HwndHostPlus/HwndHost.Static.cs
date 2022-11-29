using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using System.Threading;
using WindowEx = WinWrapper.Window;

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
                foreach (var HwndHost in HwndHosts)
                {
                    var Pt = HwndHost.TransformToVisual(HwndHost.XAMLWindow.Content).TransformPoint(
                        new Windows.Foundation.Point(0, 0)
                    );
                    var size = HwndHost.ActualSize;

                    HwndHost._CacheXFromWindow = Pt._x;
                    HwndHost._CacheYFromWindow = Pt._y;

                    HwndHost._CacheWidth = size.X;
                    HwndHost._CacheHeight = size.Y;
                }
                timer.Start();
            };
            timer.Start();
        }
        ActiveHwndHosts.Add(HwndHost);
    }
    static bool IsDwmBackdropSupported = Environment.OSVersion.Version.Build > 22621;
    static void OnHwndHostLoopCalled()
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
        => Window.ClassName is
            "Shell_TrayWnd" // Taskbar
            or "Progman" // Desktop
            or "WindowsDashboard" // I forget
            or "Windows.UI.Core.CoreWindow" // Quick Settings and Notification Center (other uwp apps should already be ApplicationFrameHost)
        ;
}
