using CommunityToolkit.Mvvm.Input;
using Get.EasyCSharp;
using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using WinUIEx.Messaging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using UnitedSets.Cells;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow
{
    #region Tabs
    [RelayCommand]
    [Event(typeof(TypedEventHandler<SplitButton, SplitButtonClickEventArgs>))]
    [Event(typeof(RoutedEventHandler))]
    private partial void OnAddTabButtonClick();

    [Event(typeof(SelectionChangedEventHandler))]
    private partial void TabSelectionChanged();


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
    private partial void OnDropOverCell(EmptyCell cell, nint hwnd);
    #endregion

    #endregion


    #region Window


    [Event(typeof(EventHandler<WindowMessageEventArgs>))]
    private partial void OnWindowMessageReceived(WindowMessageEventArgs e);

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowClosingEventArgs>))]
    private partial void OnWindowClosing(AppWindowClosingEventArgs e);

    #endregion
}
