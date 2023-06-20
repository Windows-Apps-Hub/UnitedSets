using System.Collections.ObjectModel;
using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.UI.AppWindows;


public sealed partial class MainWindow
{
    // Readonly
    public readonly WindowEx Win32Window;

    // Singleton
    readonly SettingsService Settings =
        App.Current.Services.GetService<SettingsService>() ??
        throw new InvalidOperationException();

    // Readonly
    public readonly ObservableCollection<TabBase> Tabs = new();
    public readonly ObservableCollection<TabGroup> HiddenTabs = new();
    readonly DispatcherQueueTimer timer;
    readonly WindowMessageMonitor WindowMessageMonitor;
}