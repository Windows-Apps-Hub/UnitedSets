using EasyCSharp;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using WinRT.Interop;
using WinUIEx;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using System;
using WindowRelative = WinWrapper.WindowRelative;
using WindowEx = WinWrapper.Window;
using Cursor = WinWrapper.Cursor;
using Keyboard = WinWrapper.Keyboard;
using UnitedSets.Classes;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using UnitedSets.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using System.Threading;
using System.IO;
using WinWrapper;
using System.Text.RegularExpressions;
using Windows.Foundation;
using System.Diagnostics.CodeAnalysis;
using UnitedSets.Windows.Flyout;
using UnitedSets.Classes.Tabs;
using UnitedSets.Windows.Flyout.Modules;
using CommunityToolkit.WinUI;

namespace UnitedSets.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
{

    [Event(typeof(TypedEventHandler<object, WindowActivatedEventArgs>))]
    void FirstRun()
    {
#if UNPKG
		var Package = SettingsService.Settings;
#endif
		Activated -= FirstRun;
        var icon = PInvoke.LoadImage(
            hInst: null,
            name: $@"{Package.Current.InstalledLocation.Path}\Assets\UnitedSets.ico",
            type: GDI_IMAGE_TYPE.IMAGE_ICON,
        cx: 0,
        cy: 0,
            fuLoad: IMAGE_FLAGS.LR_LOADFROMFILE | IMAGE_FLAGS.LR_DEFAULTSIZE | IMAGE_FLAGS.LR_SHARED
        );
        bool success = false;
        icon.DangerousAddRef(ref success);
        PInvoke.SendMessage(WindowEx.Handle, PInvoke.WM_SETICON, 1, icon.DangerousGetHandle());
        PInvoke.SendMessage(WindowEx.Handle, PInvoke.WM_SETICON, 0, icon.DangerousGetHandle());

        if (Keyboard.IsShiftDown)
            WindowEx.SetAppId($"UnitedSets {WindowEx.Handle}");
		Cell.ValidDrop += Cell_ValidDrop;
    }

	private void Cell_ValidDrop(object? sender, Cell.ValidItemDropArgs e) {
		var cell = sender as Cell;
		if (cell == null)
			throw new Exception("Only cells should be generating this event");
		var window = WinWrapper.Window.FromWindowHandle((nint)e.HwndId);
		var ret = PInvoke.SendMessage(window.Owner, MainWindow.UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
		var tab = Tabs.ToArray().OfType<CellTab>().FirstOrDefault(tab => tab._MainCell.AllSubCells.Any(c=>c == cell));
		if (tab == null)
			throw new Exception("Cannot find the tab parent of that cell");
		cell.RegisterWindow(new OurHwndHost(tab,this,window));
	}

	[Event(typeof(TypedEventHandler<FrameworkElement, EffectiveViewportChangedEventArgs>))]
    void OnCustomDragRegionUpdatorCalled()
    {
        CustomDragRegion.Width = CustomDragRegionUpdator.ActualWidth - 10;
        CustomDragRegion.Height = CustomDragRegionUpdator.ActualHeight;
    }

    [Event(typeof(TypedEventHandler<object, WindowSizeChangedEventArgs>))]
    void OnMainWindowResize()
    {
#if !UNPKG
		if (RootGrid.ActualWidth > 140)
			TabView.MaxWidth = RootGrid.ActualWidth - 140;
#else
		if (RootGrid.ActualWidth != 0)
			TabView.MaxWidth = RootGrid.ActualWidth;
#endif

	}

    [Event(typeof(EventHandler<WindowMessageEventArgs>))]
    void OnWindowMessageReceived(WindowMessageEventArgs e)
    {
        if (e.Message.MessageId == UnitedSetCommunicationChangeWindowOwnership)
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

    readonly AddTabFlyout AddTabFlyout = new();

    [Event(typeof(TypedEventHandler<TabView, object>))]
    async void OnAddTabButtonClick()
    {
        if (Keyboard.IsShiftDown)
        {
            LayoutManagerToggle();
		}
        else
        {
            WindowEx.Minimize();
            await AddTabFlyout.ShowAsync();
            WindowEx.Restore();
            var result = AddTabFlyout.Result;
            AddTab(result);
        }
    }
	[CommunityToolkit.Mvvm.Input.RelayCommand]
	public void LayoutManagerToggle() { 
		var newTab = new CellTab(IsAltTabVisible);
		AddTab(newTab);
		TabView.SelectedItem = newTab;
	}

    LeftFlyout? MenuFlyout;
    // Use With OpenMenu
    [MemberNotNull(nameof(MenuFlyout))]
    void CreateMeufFlyout()
    {
        MenuFlyout = LeftFlyout.CreateSingletonMode(WindowEx, new MainWindowMenuFlyoutModule(this));
        MenuFlyout.HeaderText = "";
        MenuFlyout.ExtendToTop = true;
    }
    [Event(typeof(RoutedEventHandler))]
    async void OpenMenu()
    {
        if (MenuFlyout is null or { IsDisposed : true })
        {
            CreateMeufFlyout();
        }
        try
        {
            await MenuFlyout.ShowAsync();
        } catch
        {
            CreateMeufFlyout();
            OpenMenu();
        }
    }

#pragma warning disable CA1822 // Mark members as static

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDragStartingEventArgs>))]
    void TabDragStarting(TabViewTabDragStartingEventArgs args)
    {
        if (args.Item is HwndHostTab item)
			args.Data.Properties.Add(UnitedSetsTabWindowDragProperty, (long)item.Window.Handle.Value);
	}


    [Event(typeof(DragEventHandler))]
    void OnDragItemOverTabView(DragEventArgs e)
    {
		if (e.DataView.Properties?.ContainsKey(UnitedSetsTabWindowDragProperty) == true)
			e.AcceptedOperation = DataPackageOperation.Move;
    }
#pragma warning restore CA1822 // Mark members as static

    [Event(typeof(DragEventHandler))]
    void OnDragOverTabViewItem(object sender)
    {
        if (sender is TabViewItem tvi && tvi.Tag is TabBase tb)
            TabView.SelectedIndex = Tabs.IndexOf(tb);
    }

    [Event(typeof(DragEventHandler))]
    void OnDropOverTabView(DragEventArgs e)
    {
		if (e.DataView.Properties.TryGetValue(UnitedSetsTabWindowDragProperty, out var _a) && _a is long a)
        {

            var window = WindowEx.FromWindowHandle((nint)a);
            var ret = PInvoke.SendMessage(window.Owner, UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
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

#pragma warning disable CA1822 // Mark members as static
    [Event(typeof(TypedEventHandler<TabView, TabViewTabDroppedOutsideEventArgs>))]
    void TabDroppedOutside(TabViewTabDroppedOutsideEventArgs args)
    {
        if (args.Tab.Tag is TabBase Tab)
            Tab.DetachAndDispose(JumpToCursor: true);
    }
#pragma warning restore CA1822 // Mark members as static

    [Event(typeof(SelectionChangedEventHandler))]
    void TabSelectionChanged()
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

    readonly ContentDialog ClosingWindowDialog = new()
    {
        Title = "Closing UnitedSets",
        Content = "How do you want to close the app?",
        PrimaryButtonText = "Release all Windows",
        SecondaryButtonText = "Close all Windows",
        CloseButtonText = "Cancel"
    };
	async Task TimerStop() {
		timer.Stop();
		OnTimerLoopTick();
		await Task.Delay(100);
	}

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowClosingEventArgs>))]
    async void OnWindowClosing(AppWindowClosingEventArgs e)
    {
        e.Cancel = true;//as we will just exit if we want to actually close
        ClosingWindowDialog.XamlRoot = Content.XamlRoot;
        var item = TabView.SelectedItem;
        TabView.SelectedIndex = -1;
        TabView.Visibility = Visibility.Collapsed;
        WindowEx.Focus();
		ContentDialogResult result = ContentDialogResult.Primary;
		if (Tabs.Count > 0) {
			try {
				result = await ClosingWindowDialog.ShowAsync();
			} catch {
				result = ContentDialogResult.None;
			}
		}
        switch (result)
        {
            case ContentDialogResult.Primary:
                // Release all windows
                while (Tabs.Count > 0)
                {
                    var Tab = Tabs.First();
					RemoveTab(Tab);
                    Tab.DetachAndDispose(JumpToCursor: false);
                }
				await TimerStop();

				Environment.Exit(0);
                return;
            case ContentDialogResult.Secondary:
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
					Environment.Exit(0);
                    return;
                }
                goto default;
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

    [Event(typeof(SizeChangedEventHandler))]
    void TabView_SizeChanged()
    {
        DispatcherQueue.TryEnqueue(() => TabViewSizer.InvalidateArrange());
    }
	private async void TabRemoveRequest(object? sender, EventArgs e) {
		var tab = sender as TabBase;
		if (tab == null)
			throw new ArgumentException();
		await DispatcherQueue.EnqueueAsync(() => RemoveTab(tab));
		UnwireTabEvents(tab);
	}
	private async void TabShowFlyoutRequest(object? sender, TabBase.ShowFlyoutEventArgs e) {
		var tab = sender as TabBase;
		if (tab == null)
			throw new ArgumentException();
		var flyout = new LeftFlyout(
		WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this)),
			new BasicTabFlyoutModule(tab),
				e.element
		);
		await flyout.ShowAsync();
		flyout.Close();

	}
	private void TabShowRequest(object? sender, EventArgs e) {
		var tab = sender as TabBase;
		if (tab == null)
			throw new ArgumentException();
		TabView.SelectedItem = tab;
	}
}
