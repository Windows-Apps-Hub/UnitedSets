using Microsoft.UI.Windowing;
using WinUIEx.Messaging;
using UnitedSets.Tabs;
using UnitedSets.UI.Popups;
using CommunityToolkit.Mvvm.Input;
using WinWrapper.Windowing;
using UnitedSets.Cells.Data;
using UnitedSets.PostProcessing;
using UnitedSets.Apps;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    bool _shutdownStarted;
    [RelayCommand]
    public async Task ExportData()
    {
        var res = await ExportImportInputPage.ShowExportImport(true, this);
        if (res == null)
            return;
        UnitedSetsApp.Current.Configuration.PersistantService.ExportSettings(res.FullFilename!, res.OnlyExportNonDefault);
    }
    [CommunityToolkit.Mvvm.Input.RelayCommand]
    public async Task ImportData()
    {
        var res = await ExportImportInputPage.ShowExportImport(false, this);
        if (res == null)
            return;
        await UnitedSetsApp.Current.Configuration.PersistantService.ImportSettings(res.FullFilename!);
    }

    private partial void OnDropOverCell(EmptyCell cell, nint hwnd)
    {
        var window = WindowEx.FromWindowHandle(hwnd);
        if (window.Owner == Win32Window)
        {
            if (UnitedSetsApp.Current.Tabs.ToArray().FirstOrDefault(x => x.Windows.Any(y => y == hwnd)) is TabBase Tab)
            {
                Tab.DetachAndDispose(false);
            }
        }
        else
        {
            _ = window.Owner.SendMessage(
            Constants.UnitedSetCommunicationChangeWindowOwnership, new(), window);
        }
        // TODO: actually use old tab style
        var r = PostProcessingRegisteredWindow.Register(window);
        if (r != null)
            cell.RegisterWindow(r);
    }

    private partial void TabSelectionChanged()
    {
        UnitedSetsHomeBackground.Visibility =
                UnitedSetsApp.Current.SelectedTab is null ?
                Visibility.Visible :
                Visibility.Collapsed;
        UpdateTitle();
    }
    private async Task SafeClose(TabBase tab)
    {
        try
        {
            var tsk = tab.TryCloseAsync();
            await await Task.WhenAny(tsk, Task.Delay(TimeSpan.FromSeconds(3)));//yes double await to get exceptions....
            if (tsk.IsCompletedSuccessfully)
                return;
        }
        catch (Exception)
        {
        }
        await tab.DetachAndDisposeAsync();
    }

    private partial void OnWindowClosing(AppWindowClosingEventArgs e)
    {
        e.Cancel = true;//as we will just exit if we want to actually close
        if (UnitedSetsApp.Current.Tabs.Count >= 1)
        {
            Win32Window.Focus();
            ClosingFlyout.XamlRoot = Content.XamlRoot;
            MainAreaPanel.ShowClosingFlyout(ClosingFlyout);
        }
        else
            RequestCloseAsync(UnitedSetsCloseMode.ReleaseWindow);
    }
    public async void RequestCloseAsync(UnitedSetsCloseMode closeMode)
    {
        if (_shutdownStarted) return;
        _shutdownStarted = true;
        ClosingFlyout.Hide();
        var tabs = UnitedSetsApp.Current.Tabs
            .Concat(UnitedSetsApp.Current.HiddenTabs.SelectMany(x => x.Tabs))
            .Distinct()
            .ToArray();
        switch (closeMode)
        {
            case UnitedSetsCloseMode.ReleaseWindow:
                await TimerStop();
                await Task.WhenAll(tabs.Select(x => x.DetachAndDisposeAsync(JumpToCursor: false)));
                UnitedSetsApp.Current.Tabs.Clear();
                UnitedSetsApp.Current.HiddenTabs.Clear();
                await UnitedSetsApp.Current.Suicide();
                return;
            case UnitedSetsCloseMode.CloseWindow:
                await TimerStop();
                await Task.WhenAll(tabs.Select(SafeClose));
                await UnitedSetsApp.Current.Suicide();
                return;
            case UnitedSetsCloseMode.SaveCloseWindow:
                await Task.Run(UnitedSetsApp.Current.Configuration.SaveCurrentSession);
                goto case UnitedSetsCloseMode.CloseWindow;
            default:
                throw new ArgumentOutOfRangeException(nameof(closeMode));
        }
    }
    private partial void OnWindowMessageReceived(WindowMessageEventArgs e)
    {
        var id = (WindowMessages)e.Message.MessageId;
        if (id == Constants.UnitedSetCommunicationChangeWindowOwnership)
        {
            var winPtr = e.Message.LParam;
            if (UnitedSetsApp.Current.Tabs.ToArray().FirstOrDefault(x => x.Windows.Any(y => y == winPtr)) is TabBase Tab)
            {
                Tab.DetachAndDispose(false);
                e.Result = 1;
            }
            else e.Result = 0;
            e.Handled = true;
        }
        if (id == ShellHookMessage & e.Message.WParam is /* HSHELL_FLASH */0x8006)
        {
            var winPtr = e.Message.LParam;

            if (UnitedSetsApp.Current.Tabs.ToArray().FirstOrDefault(x => x.Windows.Any(y => y == winPtr)) is TabBase Tab)
            {
                Tab.IsFlashing = true;
            }
        }
        e.Handled = false;
    }
}

public enum UnitedSetsCloseMode
{
    ReleaseWindow,
    CloseWindow,
    SaveCloseWindow
}
