using System;
using UnitedSets.Mvvm.Services;
using Microsoft.UI.Dispatching;

namespace UnitedSets.UI.AppWindows;


public sealed partial class MainWindow : NativeHelperWindow
{
    // Singleton
    readonly UnitedSetsAppSettings Settings = UnitedSetsApp.Current.Settings;

    // Readonly
    readonly DispatcherQueueTimer timer;

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
