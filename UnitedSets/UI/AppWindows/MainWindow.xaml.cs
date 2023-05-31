using EasyCSharp;
using Microsoft.UI.Windowing;
using System.Collections.ObjectModel;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using System;
using WindowEx = WinWrapper.Window;
using Windows.Win32;
using UnitedSets.Mvvm.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using UnitedSets.Classes.Tabs;
using System.Linq;
using System.Collections.Generic;
using TransparentWinUIWindowLib;

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
    readonly ObservableCollection<TabBase> Tabs = new();
    public readonly ObservableCollection<TabGroup> HiddenTabs = new();
    readonly DispatcherQueueTimer timer;
    readonly WindowMessageMonitor WindowMessageMonitor;
    readonly TransparentWindowManager? trans_mgr;
}