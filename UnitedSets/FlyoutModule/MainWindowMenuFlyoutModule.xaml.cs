using EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnitedSets.Classes;
using Windows.Foundation;
using System.Linq;
namespace UnitedSets;

public sealed partial class MainWindowMenuFlyoutModule : Grid, IWindowFlyoutModule
{
    MainWindow MainWindow;
    public MainWindowMenuFlyoutModule(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        InitializeComponent();

    }

    public void OnActivated()
    {

    }

    [Event(typeof(RoutedEventHandler))]
    void SetTabsAside()
    {
        var tabs = MainWindow.Tabs;
        var tabgroup = new TabGroup($"Tabs {DateTime.Now:hh:mm:ss}");
        foreach (var tab in tabs)
            tabgroup.Tabs.Add(tab);
        MainWindow.Tabs.Clear();
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
        MainWindow.Tabs.Add(tab);
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
            MainWindow.Tabs.Add(tab);
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
}
