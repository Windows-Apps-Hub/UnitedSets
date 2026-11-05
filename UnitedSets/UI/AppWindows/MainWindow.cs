using UnitedSets.Mvvm.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using System.Collections.Specialized;
using UnitedSets.Tabs;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// A set window: a window that hosts other windows as tabs. Several set windows can be open at once.
/// </summary>
public sealed partial class MainWindow : NativeHelperWindow
{
    // Singleton
    readonly UnitedSetsAppSettings Settings = UnitedSetsApp.Current.Settings;

    // Readonly
    readonly DispatcherQueueTimer timer;

    readonly CloseAppFlyout ClosingFlyout = new();

    /// <summary>
    /// The tabs of this set window.
    /// </summary>
    public ObservableCollection<TabBase> Tabs { get; } = [];

    void OnTabsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
            foreach (TabBase tab in e.OldItems)
                if (tab.ParentWindow == this && !Tabs.Contains(tab))
                    tab.ParentWindow = null;
        if (e.NewItems is not null)
            foreach (TabBase tab in e.NewItems)
                tab.ParentWindow = this;
        if (e.Action is NotifyCollectionChangedAction.Reset)
            foreach (var tab in Tabs)
                tab.ParentWindow = this;
        UpdateTitle();
    }

    /// <summary>
    /// Whether the window is being closed (tabs are being released/closed)
    /// </summary>
    bool isClosing;
    /// <summary>
    /// Set when the window should really close (instead of asking what to do with the tabs)
    /// </summary>
    bool allowClose;

    /// <summary>
    /// Like browsers: when the last tab is dragged out (or its window is closed) and other set windows
    /// are open, the empty set window closes itself.
    /// </summary>
    public void CloseIfEmptyAfterTabMovedOut()
    {
        if (isClosing || Tabs.Count > 0) return;
        // the last set window stays open, unless the app keeps running in the background
        bool ShouldClose() => UnitedSetsApp.Current.SetWindows.Count > 1 || UnitedSetsApp.Current.RunsInBackground;
        if (!ShouldClose()) return;
        DispatcherQueue.TryEnqueue(() =>
        {
            if (isClosing || Tabs.Count > 0) return;
            if (!ShouldClose()) return;
            CloseThisSetWindow();
        });
    }

    /// <summary>
    /// Closes this set window without touching the tabs (they must have been released before).
    /// Exits the app if this is the last set window.
    /// </summary>
    internal async void CloseThisSetWindow()
    {
        isClosing = true;
        if (UnitedSetsApp.Current.SetWindows.Count(w => w != this) is 0 && !UnitedSetsApp.Current.RunsInBackground)
        {
            await TimerStop();
            await UnitedSetsApp.Current.Suicide();
            return;
        }
        timer.Stop();
        TabBase.OnUpdateStatusLoopComplete -= OnDifferentThreadLoop;
        taskbarTabs.Dispose();
        UnitedSetsApp.Current.UnregisterSetWindow(this);
        allowClose = true;
        Close();
    }

    public void Maximize()
    {
        if (AppWindow.Presenter is OverlappedPresenter presenter)
            presenter.Maximize();
    }

    public void ToggleMaximize()
    {
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            if (presenter.State is OverlappedPresenterState.Maximized)
                presenter.Restore();
            else
                presenter.Maximize();
        }
    }

    /// <summary>
    /// Selects the next (or previous) tab, wrapping around.
    /// </summary>
    public void SelectAdjacentTab(int direction)
    {
        var count = Tabs.Count;
        if (count is 0) return;
        var index = SelectedTab is { } tab ? Tabs.IndexOf(tab) : -1;
        index = ((index + direction) % count + count) % count;
        SelectedTab = Tabs[index];
    }

    /// <summary>
    /// Selects the tab at the given index (0-based). <c>-1</c> selects the last tab.
    /// </summary>
    public void SelectTabAt(int index)
    {
        if (Tabs.Count is 0) return;
        if (index < 0 || index >= Tabs.Count) index = Tabs.Count - 1;
        SelectedTab = Tabs[index];
    }
}
