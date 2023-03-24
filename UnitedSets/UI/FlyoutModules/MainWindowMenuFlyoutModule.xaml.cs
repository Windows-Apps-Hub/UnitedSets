using EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnitedSets.Classes;
using Windows.Foundation;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Window = WinWrapper.Window;
using UnitedSets.Classes.Tabs;
using UnitedSets.UI.AppWindows;
using System.ComponentModel;

namespace UnitedSets.UI.FlyoutModules;

public sealed partial class MainWindowMenuFlyoutModule : Grid, INotifyPropertyChanged
{
    [AutoNotifyProperty]
    MainWindow? _MainWindow;
#pragma warning disable CS0067
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    public MainWindowMenuFlyoutModule()
    {
        InitializeComponent();
    }


    [Event(typeof(RoutedEventHandler))]
    void SetTabsAside()
    {
        
        var tabgroup = new TabGroup($"Tabs {DateTime.Now:hh:mm:ss}");
		var tabs = MainWindow.GetTabsAndClear();
		foreach (var tab in tabs)
            tabgroup.Tabs.Add(tab);
        MainWindow.HiddenTabs.Add(tabgroup);
    }
    [Event(typeof(SelectionChangedEventHandler))]
    void GroupSelectionChanged()
    {
        if (TabGroupListView.SelectedItem is TabGroup TabGroup)
            TabListView.ItemsSource = TabGroup.Tabs;
    }
    [Event(typeof(RoutedEventHandler))]
    void ShowOnWindow([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not TabBase tab) return;
        MainWindow.AddTab(tab);
        if (TabGroupListView.SelectedItem is TabGroup TabGroup)
            TabGroup.Tabs.Remove(tab);
    }
    [Event(typeof(RoutedEventHandler))]
    void ShowGroupOnWindow([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not TabGroup tabgroup) return;
        MainWindow.HiddenTabs.Remove(tabgroup);
        foreach (var tab in tabgroup.Tabs)
        {
            MainWindow.AddTab(tab);
        }
    }
    [Event(typeof(RoutedEventHandler))]
    async void OpenContentDialogTag([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not ContentDialog dialog) return;
        dialog.XamlRoot = XamlRoot;
        await dialog.ShowAsync();
    }
    [Event(typeof(RoutedEventHandler))]
    void DetachTab([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not TabBase tab) return;
        tab.DetachAndDispose();
        //if (TabGroupListView.SelectedItem is TabGroup TabGroup)
        //    TabGroup.Tabs.Remove(tab);
    }
    [Event(typeof(RoutedEventHandler))]
    void DetachTabGroup([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not TabGroup tabgroup) return;
        foreach (var tab in tabgroup.Tabs)
            tab.DetachAndDispose();
        //MainWindow.HiddenTabs.Remove(tabgroup);
    }
    [Event(typeof(RoutedEventHandler))]
    async void CloseTab([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not TabBase tab) return;
        var cancelationTokenSource = new CancellationTokenSource(2000);
        var task = tab.TryCloseAsync();
        var delayTask = Task.Delay(2000, cancelationTokenSource.Token);
        _ = task.ContinueWith(x => cancelationTokenSource.Cancel());
        await delayTask;

        if (!task.IsCompleted)
            switch (await new ContentDialog
            {
                Title = "Warning",
                Content = "You might need to take some action to close the window (Did the window ask you to save the file or something?)",
                PrimaryButtonText = "Open Window",
                SecondaryButtonText = "Detach Window"
            }.ShowAsync())
            {
                case ContentDialogResult.Primary:
                    ShowOnWindow(sender);
                    break;
                case ContentDialogResult.Secondary:
                    DetachTab(sender);
                    break;
            }
        if (TabGroupListView.SelectedItem is TabGroup TabGroup)
            TabGroup.Tabs.Remove(tab);
    }
    [Event(typeof(RoutedEventHandler))]
    async void CloseTabGroup([CastFrom(typeof(object))] FrameworkElement sender)
    {
        if (sender.Tag is not TabGroup tabGroup) return;
        var cancelationTokenSource = new CancellationTokenSource(2000);
        var task = Task.WhenAll(tabGroup.Tabs.Select(tab => tab.TryCloseAsync()));
        var delayTask = Task.Delay(2000, cancelationTokenSource.Token);
        _ = task.ContinueWith(x => cancelationTokenSource.Cancel());
        await delayTask;

        if (!task.IsCompleted)
            switch (await new ContentDialog
            {
                Title = "Warning",
                Content = "You might need to take some action to close the windows (Did the window ask you to save the file or something?)",
                PrimaryButtonText = "Open Window",
                SecondaryButtonText = "Detach Window"
            }.ShowAsync())
            {
                case ContentDialogResult.Primary:
                    ShowOnWindow(sender);
                    break;
                case ContentDialogResult.Secondary:
                    DetachTab(sender);
                    break;
            }
    }

#pragma warning disable CA1822 // Mark members as static
    [Event(typeof(DragItemsStartingEventHandler))]
    void TabDragStarting(DragItemsStartingEventArgs args)
    {
        if (args.Items.Count is not 0) return;
        if (args.Items[0] is HwndHostTab item)
            args.Data.Properties.Add(MainWindow.UnitedSetsTabWindowDragProperty, (long)item.Window.Handle.Value);
    }


    [Event(typeof(DragEventHandler))]
    void OnDragItemOverTabListView(DragEventArgs e)
    {
        if (e.DataView.Properties.ContainsKey(MainWindow.UnitedSetsTabWindowDragProperty))
            e.AcceptedOperation = DataPackageOperation.Move;
    }
#pragma warning restore CA1822 // Mark members as static
    [Event(typeof(DragEventHandler))]
    void OnDragItemOverTabGroupListViewItem([CastFrom(typeof(object))] ListViewItem listviewitem)
    {
        TabListView.SelectedItem = listviewitem.Tag;
    }
    [Event(typeof(DragEventHandler))]
    void OnDropItemOverTabListView(DragEventArgs e)
    {
        const string UnitedSetsTabWindowDragProperty = MainWindow.UnitedSetsTabWindowDragProperty;

		if (!e.DataView.Properties.TryGetValue(UnitedSetsTabWindowDragProperty, out var _a) || _a is long a == false)
			return;
		
		var pt = e.GetPosition(TabListView);
        if (TabGroupListView.SelectedIndex is -1)
			return;
        var tabgroup = MainWindow.HiddenTabs[TabGroupListView.SelectedIndex];
        var window = Window.FromWindowHandle((nint)a);
        var finalIdx = (
            from index in Enumerable.Range(0, tabgroup.Tabs.Count)
            let ele = TabListView.ContainerFromIndex(index) as UIElement
            let posele = ele.TransformToVisual(TabListView).TransformPoint(default)
            let size = ele.ActualSize
            let IsMoreThanTopLeft = pt.X >= posele.X && pt.Y >= posele.Y
            let IsLessThanBotRigh = pt.X <= posele.X + size.X && pt.Y <= posele.Y + size.Y
            where IsMoreThanTopLeft && IsLessThanBotRigh
            select (int?)index
        ).FirstOrDefault();
        TabBase? tabValue;
		if (window.Owner != MainWindow.WindowEx) {
			var ret = PInvoke.SendMessage(window.Owner, MainWindow.UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
			tabValue = MainWindow.JustCreateTab(window);
		} else {
			tabValue = MainWindow.FindTabByWindow(window);
			if (tabValue != null)
				MainWindow.RemoveTab(tabValue);

			var hiddenResult = MainWindow.FindHiddenTabByWindow(window);
			if (hiddenResult.tab != null) {
				tabValue = hiddenResult.tab;
				hiddenResult.group!.Tabs.Remove(tabValue);
			}
		}
		if (tabValue is null) return;
		if (finalIdx.HasValue)
			tabgroup.Tabs.Insert(finalIdx.Value, tabValue);
		else tabgroup.Tabs.Add(tabValue);
	}
}
