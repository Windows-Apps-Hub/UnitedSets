﻿using Microsoft.UI.Dispatching;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitedSets.Tabs;
using UnitedSets.UI.AppWindows;
using Window = WinWrapper.Windowing.Window;
using System;
using Get.EasyCSharp;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Threading.Tasks;
using UnitedSets.Services;
using UnitedSets.Classes;
using UnitedSets.Mvvm.Services;

namespace UnitedSets;

partial class UnitedSetsApp : INotifyPropertyChanged
{
    // singleton setup
    private UnitedSetsApp() {
        Settings = new();
        Configuration = new();
    }
    public UnitedSetsAppSettings Settings { get; } = new();
    public UnitedSetsAppConfiguration Configuration { get; } = new();
    public static UnitedSetsApp Current { get; } = new();
    readonly List<Window> _allWindows = [];
    
    public IReadOnlyList<Window> AllUnitedSetsWindows => _allWindows;
    public MainWindow MainWindow { get; private set; } = null!;
    public DispatcherQueue DispatcherQueue { get; private set; } = null!;
    public ObservableCollection<TabBase> Tabs { get; } = [];
    public ObservableCollection<TabGroup> HiddenTabs { get; } = [];
    [AutoNotifyProperty]
    TabBase? _SelectedTab;
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public void RegisterUnitedSetsWindow(Window window) => _allWindows.Add(window);
    public void RegisterUnitedSetsWindow(MainWindow window)
    {
        MainWindow = window;
        DispatcherQueue = window.DispatcherQueue;
        RegisterUnitedSetsWindow(Window.FromWindowHandle((nint)window.AppWindow.Id.Value));
    }
    
    [DoesNotReturn]
    public async Task Suicide()
    {
        //trans_mgr?.Cleanup();
        await Task.Delay(300);
        Debug.WriteLine("Cleanish exit");
        Environment.Exit(0);
    }

}
