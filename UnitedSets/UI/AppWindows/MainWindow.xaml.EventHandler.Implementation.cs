using EasyCSharp;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using System;
using WindowEx = WinWrapper.Window;
using Keyboard = WinWrapper.Keyboard;
using UnitedSets.Classes;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using System.ComponentModel;
using System.Diagnostics;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using Windows.Foundation;
using UnitedSets.Classes.Tabs;
using CommunityToolkit.WinUI;
using OutOfBoundsFlyout;
using Microsoft.UI.Input;
using UnitedSets.UI.Popups;
using UnitedSets.UI.FlyoutModules;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
{
	private partial void OnCustomDragRegionUpdatorCalled()
    {
        CustomDragRegion.Width = CustomDragRegionUpdator.ActualWidth - 10;
        CustomDragRegion.Height = CustomDragRegionUpdator.ActualHeight;
    }

    private partial void OnMainWindowResize()
    {
#if !UNPKG
		if (RootGrid.ActualWidth > 140)
			TabView.MaxWidth = RootGrid.ActualWidth - 140;
#else
		if (RootGrid.ActualWidth != 0)
			TabView.MaxWidth = RootGrid.ActualWidth;
#endif
	}

    readonly AddTabPopup AddTabPopup = new();
    private async partial void OnAddTabButtonClick()
    {
        if (Keyboard.IsShiftDown)
        {
            AddSplitableTab();
		}
        else
        {
            Win32Window.Minimize();
            await AddTabPopup.ShowAsync();
            Win32Window.Restore();
            var result = AddTabPopup.Result;
            AddTab(result);
        }
    }

    private partial void AddSplitableTab()
    {
        var newTab = new CellTab(Constants.IsAltTabVisible);
        AddTab(newTab);
        TabView.SelectedItem = newTab;
    }

    private partial void TabDragStarting(TabViewTabDragStartingEventArgs args)
    {
        if (args.Item is HwndHostTab item)
            args.Data.Properties.Add(Constants.UnitedSetsTabWindowDragProperty, (long)item.Window.Handle.Value);
    }

    private partial void OnDragItemOverTabView(DragEventArgs e)
    {
        if (e.DataView.Properties?.ContainsKey(Constants.UnitedSetsTabWindowDragProperty) == true)
            e.AcceptedOperation = DataPackageOperation.Move;
    }

    public partial void OnDragOverTabViewItem(object sender)
    {
        if (sender is FrameworkElement tvi && tvi.Tag is TabBase tb)
            TabView.SelectedIndex = Tabs.IndexOf(tb);
    }

    private partial void OnDropOverTabView(DragEventArgs e)
    {
        if (e.DataView.Properties.TryGetValue(Constants.UnitedSetsTabWindowDragProperty, out var _a) && _a is long a)
        {

            var window = WindowEx.FromWindowHandle((nint)a);
            var ret = PInvoke.SendMessage(window.Owner, Constants.UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
            var pt = e.GetPosition(TabView);
            var finalIdx = (
                from index in Enumerable.Range(0, Tabs.Count)
                let ele = TabView.ContainerFromIndex(index) as UIElement
                let posele = ele.TransformToVisual(TabView).TransformPoint(default)
                let size = ele.ActualSize
                let IsMoreThanTopLeft = pt.X >= posele.X && pt.Y >= posele.Y
                let IsLessThanBotRigh = pt.X <= posele.X + size.X && pt.Y <= posele.Y + size.Y
                where IsMoreThanTopLeft && IsLessThanBotRigh
                select (int?)index
            ).FirstOrDefault();
            AddTab(window, finalIdx);
        }
    }

    private partial void TabDroppedOutside(TabViewTabDroppedOutsideEventArgs args)
    {
        if (args.Tab.Tag is TabBase Tab)
            Tab.DetachAndDispose(JumpToCursor: true);
    }

    private partial void TabSelectionChanged()
    {
        UnitedSetsHomeBackground.Visibility =
                TabView.SelectedIndex != -1 && Tabs[TabView.SelectedIndex] is CellTab ?
                Visibility.Collapsed :
                Visibility.Visible;

        if (TabView.SelectedIndex is not -1)
        {
            Title = $"{Tabs[TabView.SelectedIndex].Title} (+{Tabs.Count - 1} Tabs) - United Sets";
        }
        else
        {
            Title = "United Sets";
        }
    }

    private partial void TabView_SizeChanged()
    {
        DispatcherQueue.TryEnqueue(() => TabViewSizer.InvalidateArrange());
    }
    
    private async partial void TabRemoveRequest(TabBase tab)
    {
        await DispatcherQueue.EnqueueAsync(() => RemoveTab(tab));
        UnwireTabEvents(tab);
    }

    private async partial void TabShowFlyoutRequest(TabBase tab, TabBase.ShowFlyoutEventArgs e)
    {
        await Task.Delay(300);
        AttachedOutOfBoundsFlyout.ShowFlyout(
            e.RelativeTo,
            new Flyout
            {
                Content = new StackPanel
                {
                    Width = 350,
                    Spacing = 8,
                    Children =
                    {
                        new BasicTabFlyoutModule(tab),
                        e.Element
                    }
                }
            },
            e.CursorPosition,
            e.PointerDeviceType is not (PointerDeviceType.Touchpad or PointerDeviceType.Mouse)
        );

    }
    
    private partial void TabShowRequest(TabBase tab, EventArgs e)
        => TabView.SelectedItem = tab;

    readonly ContentDialog ClosingWindowDialog = new()
    {
        Title = "Closing UnitedSets",
        Content = "How do you want to close the app?",
        PrimaryButtonText = "Release all Windows",
        SecondaryButtonText = "Close all Windows",
        CloseButtonText = "Cancel"
    };
    
    private async partial void OnWindowClosing(AppWindowClosingEventArgs e)
    {
        e.Cancel = true;//as we will just exit if we want to actually close
        ClosingWindowDialog.XamlRoot = Content.XamlRoot;
        var item = TabView.SelectedItem;
        TabView.SelectedIndex = -1;
        TabView.Visibility = Visibility.Collapsed;
        Win32Window.Focus();
        ContentDialogResult result = ContentDialogResult.Primary;
        if (Tabs.Count > 0)
        {
            try
            {
                result = await ClosingWindowDialog.ShowAsync();
            }
            catch
            {
                result = ContentDialogResult.None;
            }
        }
        switch (result)
        {
            case ContentDialogResult.Primary:
                await RequestCloseAsync(CloseMode.ReleaseWindow);
                break;
            case ContentDialogResult.Secondary:
                await RequestCloseAsync(CloseMode.CloseWindow);
                break;
            default:
                // Do not close window
                try
                {
                    TabView.SelectedItem = item;
                }
                catch
                {
                    if (Tabs.Count > 0)
                        TabView.SelectedIndex = 0;
                }
                TabView.Visibility = Visibility.Visible;
                break;
        }
    }
    public enum CloseMode
    {
        ReleaseWindow,
        CloseWindow
    }
    [DoesNotReturn]
    public async Task RequestCloseAsync(CloseMode closeMode)
    {
        switch (closeMode)
        {
            case CloseMode.ReleaseWindow:
                // Release all windows
                while (Tabs.Count > 0)
                {
                    var Tab = Tabs.First();
                    RemoveTab(Tab);
                    Tab.DetachAndDispose(JumpToCursor: false);
                }
                await TimerStop();

                await Suicide();

                return;
            case CloseMode.CloseWindow:
                // Close all windows
                TabView.Visibility = Visibility.Visible;
                await Task.Delay(100);
                foreach (var Tab in Tabs.ToArray()) // ToArray = Clone Collection
                {
                    try
                    {
                        _ = Tab.TryCloseAsync();
                        // Try closing tab in 3 second, otherwise give up
                        for (int i = 0; i < 30; i++)
                        {
                            await Task.Delay(100);
                            if (!Tab.IsDisposed) continue;
                        }
                        if (!Tab.IsDisposed) break;
                    }
                    catch
                    {
                        Tab.DetachAndDispose(JumpToCursor: false);
                    }
                }
                if (Tabs.Count == 0)
                {
                    await TimerStop();
                    await Suicide();

                    return;
                }
                goto default;
            default:
                throw new ArgumentOutOfRangeException(nameof(closeMode));
        }
    }
    private partial void CellWindowDropped(Cell cell, nint HwndId)
    {
        if (cell == null)
            throw new Exception("Only cells should be generating this event");
        var window = WindowEx.FromWindowHandle(HwndId);
        var ret = PInvoke.SendMessage(
            window.Owner,
            Constants.UnitedSetCommunicationChangeWindowOwnership,
            new(),
            new(window)
        );
        var tab =
            Tabs.ToArray().OfType<CellTab>()
            .First(tab => tab._MainCell.AllSubCells.Any(c => c == cell));

        cell.RegisterHwndHost(new OurHwndHost(tab, this, window));
    }
    private partial void OnWindowMessageReceived(WindowMessageEventArgs e)
    {
        var id = e.Message.MessageId;
        if (id == Constants.UnitedSetCommunicationChangeWindowOwnership)
        {
            var winPtr = e.Message.LParam;
            if (Tabs.ToArray().FirstOrDefault(x => x.Windows.Any(y => y == winPtr)) is TabBase Tab)
            {
                Tab.DetachAndDispose(false);
                e.Result = 1;
            }
            else e.Result = 0;
        }
    }
}
