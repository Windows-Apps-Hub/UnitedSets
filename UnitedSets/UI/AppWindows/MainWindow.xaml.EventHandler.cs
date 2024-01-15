using CommunityToolkit.Mvvm.Input;
using Get.EasyCSharp;
using System;
using Microsoft.UI.Xaml;
using UnitedSets.Classes;
using Windows.Foundation;
using WinUIEx.Messaging;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes.Tabs;
using Microsoft.UI.Windowing;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    #region Tabs
    [RelayCommand]
    [Event(typeof(RoutedEventHandler))]
    private partial void OnAddTabButtonClick();

    [RelayCommand]
    private partial void AddSplitableTab();

    [Event(typeof(SelectionChangedEventHandler))]
    private partial void TabSelectionChanged();

    [Event(typeof(EventHandler))]
    private partial void TabRemoveRequest([CastFrom(typeof(object))] TabBase tab);

    [Event(typeof(EventHandler<TabBase.ShowFlyoutEventArgs>))]
    private partial void TabShowFlyoutRequest([CastFrom(typeof(object))] TabBase tab, TabBase.ShowFlyoutEventArgs e);

    [Event(typeof(EventHandler))]
    private partial void TabShowRequest([CastFrom(typeof(object))] TabBase tab, EventArgs e);

    #region Tabs Dragging

    [Event(typeof(DragEventHandler))]
    public partial void OnDragOverTabViewItem(object sender);

    [Event(typeof(DragEventHandler))]
    private partial void OnDragItemOverTabView(DragEventArgs e);
    
    [Event(typeof(DragEventHandler))]
    private partial void OnDropOverTabView(DragEventArgs e);

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDragStartingEventArgs>))]
    private partial void TabDragStarting(TabViewTabDragStartingEventArgs args);
    
    [Event(typeof(TypedEventHandler<TabView, TabViewTabDroppedOutsideEventArgs>))]
    private partial void TabDroppedOutside(TabViewTabDroppedOutsideEventArgs args);

    #endregion

    #endregion

    
    #region Window

    
    [Event(typeof(EventHandler<WindowMessageEventArgs>))]
    private partial void OnWindowMessageReceived(WindowMessageEventArgs e);

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowClosingEventArgs>))]
    private partial void OnWindowClosing(AppWindowClosingEventArgs e);

    #endregion

    private partial void CellWindowDropped(Cell cell, nint HwndId);
}
