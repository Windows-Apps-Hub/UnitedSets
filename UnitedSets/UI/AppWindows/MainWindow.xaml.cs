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


public sealed partial class MainWindow : INotifyPropertyChanged
{
    // Constant/Static Readonly
    public const string UnitedSetsTabWindowDragProperty = "UnitedSetsTabWindow";
    public static readonly bool IsAltTabVisible = false;
    public static readonly uint UnitedSetCommunicationChangeWindowOwnership
        = PInvoke.RegisterWindowMessage(nameof(UnitedSetCommunicationChangeWindowOwnership));

    // Readonly
    public readonly ObservableCollection<TabBase> Tabs = new();
    public readonly ObservableCollection<TabGroup> HiddenTabs = new();
    public readonly WindowEx WindowEx;

    // Implement INotifyPropertyChanged
    PropertyChangedEventHandler? _PropertyChanged;
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => _PropertyChanged += value;
        remove => _PropertyChanged -= value;
    }

    // Singleton
    readonly SettingsService Settings =
        App.Current.Services.GetService<SettingsService>() ??
        throw new InvalidOperationException();

    // Readonly
    readonly DispatcherQueueTimer timer;
    readonly WindowMessageMonitor WindowMessageMonitor;
    readonly TransparentWindowManager? trans_mgr;
    
    // Readonly Property
    [Property(
        PropertyName = "HasOwner",
        CustomGetExpression = $"{nameof(cacheHasOwner)} = {nameof(WindowEx)}.Owner.IsValid",
        SetVisibility = GeneratorVisibility.DoNotGenerate
    )]
    bool cacheHasOwner = false;

    
}