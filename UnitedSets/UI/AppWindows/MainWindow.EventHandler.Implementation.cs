using Microsoft.UI.Windowing;
using WinUIEx.Messaging;
using UnitedSets.Tabs;
using UnitedSets.UI.Popups;
using CommunityToolkit.Mvvm.Input;
using WinWrapper.Windowing;
using UnitedSets.Cells.Data;
using UnitedSets.PostProcessing;
using UnitedSets.Apps;
using UnitedSets.Mvvm.Services;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
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
        => _ = DropWindowIntoCellAsync(cell, WindowEx.FromWindowHandle(hwnd));

    /// <summary>
    /// Puts a window into an empty cell of a splitable tab, taking it out of its current tab first.
    /// </summary>
    internal static async Task DropWindowIntoCellAsync(EmptyCell cell, WindowEx window)
    {
        if (UnitedSetsApp.Current.FindTab(window.Handle) is { Tab: WindowHostTab tab })
            // wait for the window to be fully released before registering it again,
            // otherwise its "initial" styles would be captured while still modified
            await tab.DetachAndDisposeAsync(JumpToCursor: false);
        else if (window.Owner.IsValid && UnitedSetsApp.Current.FindSetWindowOf(window.Owner) is null)
            // owned by another United Sets process
            _ = window.Owner.SendMessage(
                Constants.UnitedSetCommunicationChangeWindowOwnership, new(), window);
        var r = PostProcessingRegisteredWindow.Register(window);
        if (r != null)
            cell.RegisterWindow(r);
    }

    private partial void TabSelectionChanged()
    {
        UpdateHomeVisibility();
        UpdateTitle();
    }
    /// <summary>
    /// The home page (tips) is only shown when no tab is selected. It used to stay visible behind tabbed windows
    /// and, on small windows, showed through the tab strip.
    /// </summary>
    void UpdateHomeVisibility()
        => UnitedSetsHomeBackground.Visibility = SelectedTab is null ? Visibility.Visible : Visibility.Collapsed;

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
        tab.DetachAndDispose();
    }

    private partial void OnWindowClosing(AppWindowClosingEventArgs e)
    {
        if (allowClose) return;
        e.Cancel = true;//as we will just exit if we want to actually close
        if (isClosing) return;
        if (Tabs.Count >= 1)
        {
            switch (Settings.CloseSetWithTabsBehavior.Value)
            {
                case CloseSetWithTabsBehaviors.ReleaseWindows:
                    RequestCloseAsync(UnitedSetsCloseMode.ReleaseWindow);
                    return;
                case CloseSetWithTabsBehaviors.CloseWindows:
                    RequestCloseAsync(UnitedSetsCloseMode.CloseWindow);
                    return;
            }
            Win32Window.Focus();
            ClosingFlyout.XamlRoot = Content.XamlRoot;
            MainAreaPanel.ShowClosingFlyout(ClosingFlyout);
        }
        else
            RequestCloseAsync(UnitedSetsCloseMode.ReleaseWindow);
    }
    /// <summary>
    /// Closes this set window. Its tabs are released or closed depending on <paramref name="closeMode"/>.
    /// The app exits when the last set window is closed.
    /// </summary>
    public async void RequestCloseAsync(UnitedSetsCloseMode closeMode)
    {
        ClosingFlyout.Hide();
        if (isClosing) return;
        isClosing = true;
        try
        {
            switch (closeMode)
            {
                case UnitedSetsCloseMode.ReleaseWindow:
                    // Release all windows
                    while (Tabs.Count > 0)
                    {
                        var Tab = Tabs.First();
                        Tabs.Remove(Tab);
                        Tab.DetachAndDispose(JumpToCursor: false);
                    }
                    break;
                case UnitedSetsCloseMode.CloseWindow:
                    // Close all windows
                    await Task.Delay(100);
                    await Task.WhenAll(Tabs.ToArray().Select(SafeClose));
                    break;
                case UnitedSetsCloseMode.SaveCloseWindow:
                    // The session contains every set window, so save and close all of them
                    UnitedSetsApp.Current.Configuration.SaveCurrentSession();
                    await Task.Delay(100);
                    await Task.WhenAll(UnitedSetsApp.Current.AllTabs.ToArray().Select(tab =>
                        tab.ParentWindow is { } w ? w.SafeClose(tab) : Task.CompletedTask));
                    // the session is restored the next time United Sets starts; it keeps running in the tray
                    foreach (var other in UnitedSetsApp.Current.SetWindows.ToArray())
                        if (other != this)
                            other.CloseThisSetWindow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(closeMode));
            }
        }
        catch (Exception ex)
        {
            Helpers.CrashLog.Write("Closing set window", ex);
        }
        CloseThisSetWindow();
    }
    private partial void OnWindowMessageReceived(WindowMessageEventArgs e)
    {
        if (e.Message.MessageId is 0x0216 /* WM_MOVING */)
        {
            // move the tabbed window together with the set (instead of one step behind it)
            var proposed = System.Runtime.InteropServices.Marshal.PtrToStructure<MovingRect>((nint)e.Message.LParam);
            WindowHoster.RegisteredWindowController.OnHostMoved(Win32Window.Handle, proposed.Left, proposed.Top);
            return;
        }
        if (e.Message.MessageId is 0x0047 /* WM_WINDOWPOSCHANGED */)
        {
            // any move (dragging, snapping, moving to another monitor, other programs...)
            var pos = System.Runtime.InteropServices.Marshal.PtrToStructure<WindowPos>((nint)e.Message.LParam);
            if ((pos.Flags & 0x2 /* SWP_NOMOVE */) == 0)
                WindowHoster.RegisteredWindowController.OnHostMoved(Win32Window.Handle, pos.X, pos.Y);
            return;
        }
        var id = (WindowMessages)e.Message.MessageId;
        if (id == Constants.UnitedSetCommunicationChangeWindowOwnership)
        {
            // another United Sets process wants one of our windows
            var winPtr = (nint)e.Message.LParam;
            if (Tabs.ToArray().FirstOrDefault(x => x is WindowHostTab && x.ContainsWindow(winPtr)) is TabBase Tab)
            {
                Tab.DetachAndDispose(false);
                e.Result = 1;
            }
            else e.Result = 0;
            e.Handled = true;
            return;
        }
        if (ShellHookMessage != default && id == ShellHookMessage && e.Message.WParam is /* HSHELL_FLASH */0x8006)
        {
            var winPtr = (nint)e.Message.LParam;

            if (Tabs.ToArray().FirstOrDefault(x => x.ContainsWindow(winPtr)) is TabBase Tab)
            {
                Tab.IsFlashing = true;
            }
        }
        e.Handled = false;
    }
}

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
struct MovingRect { public int Left, Top, Right, Bottom; }
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
struct WindowPos { public nint Hwnd, HwndInsertAfter; public int X, Y, Cx, Cy; public uint Flags; }

public enum UnitedSetsCloseMode
{
    ReleaseWindow,
    CloseWindow,
    SaveCloseWindow
}
