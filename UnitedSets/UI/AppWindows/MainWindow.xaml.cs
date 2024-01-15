using System.Collections.ObjectModel;
using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.UI.AppWindows;


public sealed partial class MainWindow : WinUIEx.WindowEx
{
    // Readonly
    public readonly WindowEx Win32Window;

    // Singleton
    readonly SettingsService Settings = App.SettingsService;

    // Readonly
    public readonly ObservableCollection<TabBase> Tabs = new();
    public readonly ObservableCollection<TabGroup> HiddenTabs = new();
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