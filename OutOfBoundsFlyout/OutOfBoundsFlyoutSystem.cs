using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Point = System.Drawing.Point;
using System.Collections.Generic;
using Microsoft.UI.Dispatching;

namespace OutOfBoundsFlyout;

public static class OutOfBoundsFlyoutSystem
{
    readonly static Dictionary<DispatcherQueue, OutOfBoundsFlyoutHost> HostDictionary = new();
    
    public static async Task ShowAsync(FlyoutBase Flyout, Point pt, bool TouchPenCompatibilityMode, FlyoutPlacementMode placementMode = FlyoutPlacementMode.Auto, Rect? ExclusionRect = default)
    {
        var dispatcherQueue = Flyout.DispatcherQueue;
        if (!dispatcherQueue.HasThreadAccess) throw new ArgumentOutOfRangeException(nameof(dispatcherQueue), "Please invoke ShowAsync from the thread that the flyout is created on.");

        var instance = GetHost(dispatcherQueue);
        await instance.ShowFlyoutAsync(Flyout, pt, TouchPenCompatibilityMode, placementMode, ExclusionRect);
    }

    static OutOfBoundsFlyoutHost GetHost(DispatcherQueue dispatcherQueue)
    {
        if (HostDictionary.TryGetValue(dispatcherQueue, out var host)) return host;
        host = new(true);
        HostDictionary.Add(dispatcherQueue, host);
        return host;
    }

    public static void CloseFlyout()
    {
        foreach (var (dq, host) in HostDictionary)
        {
            if (!dq.HasThreadAccess) continue;
            host.CloseFlyout();
            break;
        }
    }

    public static void Dispose()
    {
        foreach (var host in HostDictionary.Values)
        {
            host.Dispose();
        }
        HostDictionary.Clear();
    }
}

