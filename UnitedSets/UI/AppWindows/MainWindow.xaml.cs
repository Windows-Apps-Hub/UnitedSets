using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Mvvm.Services;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;

namespace UnitedSets.UI.AppWindows;


public sealed partial class MainWindow : WinUIEx.WindowEx
{
    // Readonly
    public readonly WindowEx Win32Window;

    // Singleton
    readonly UnitedSetsAppSettings Settings = UnitedSetsApp.Current.Settings;

    // Readonly
    readonly DispatcherQueueTimer timer;
    readonly WindowMessageMonitor WindowMessageMonitor;

    DateTime LatestUpdate;
    private void DragRegion_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (DateTime.UtcNow - LatestUpdate < TimeSpan.FromSeconds(1))
        {
            LatestUpdate = DateTime.UtcNow;
            return;
        }
        DragRegion.UpdateRegion();
        LatestUpdate = DateTime.UtcNow;
    }
}