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

namespace UnitedSets;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
{
    public static readonly uint UnitedSetCommunicationChangeWindowOwnership
        = PInvoke.RegisterWindowMessage(nameof(UnitedSetCommunicationChangeWindowOwnership));

    public SettingsService Settings = App.Current.Services.GetService<SettingsService>() ?? throw new InvalidOperationException();
    public ObservableCollection<TabBase> Tabs { get; } = new();
    readonly WindowEx WindowEx;
    bool _HasOwner = false;
    Visibility SettingsButtonVisibility => HasOwner ? Visibility.Collapsed : Visibility.Visible;
    public bool HasOwner
    {
        get => WindowEx.Owner.IsValid;
    }
    DispatcherQueueTimer timer;
    WindowMessageMonitor WindowMessageMonitor;
    public System.Drawing.Rectangle CacheMiddleAreaBounds { get; private set; }
    public MainWindow()
    {
        Title = "UnitedSets";
        InitializeComponent();
        WindowEx = WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this));
        TabBase.MainWindows.Add(WindowEx);
        AppWindow.Closing += OnWindowClosing;
        Activated += FirstRun;
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(CustomDragRegion);
        TabBase.OnUpdateStatusLoopComplete += OnLoopCalled;
        WindowMessageMonitor = new WindowMessageMonitor(WindowEx);
        WindowMessageMonitor.WindowMessageReceived += MainWindow_WindowMessageReceived;
        MinWidth = 100;

        timer = DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Tick += delegate
        {
            var Pt = MainAreaBorder.TransformToVisual(Content).TransformPoint(
                new Windows.Foundation.Point(0, 0)
            );
            var size = MainAreaBorder.ActualSize;
            CacheMiddleAreaBounds = new System.Drawing.Rectangle((int)Pt._x, (int)Pt._y, (int)size.X, (int)size.Y);
            var idx = TabView.SelectedIndex;
            SelectedTabCache = idx < 0 ? null : Tabs[idx];
        };
        timer.Start();
        TabView.SelectionChanged += delegate
        {
            UnitedSetsHomeBackground.Visibility =
                TabView.SelectedIndex != -1 && Tabs[TabView.SelectedIndex] is CellTab ?
                Visibility.Collapsed :
                Visibility.Visible;
        };
        //AfterInit();
    }
    TabBase? SelectedTabCache;
    //private async void AfterInit()
    //{
    //    await Task.Delay(1000);
    //    Tabs.Add(new CellTab(this));
    //}
    private void MainWindow_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        if (e.Message.MessageId == UnitedSetCommunicationChangeWindowOwnership)
        {
            var winPtr = e.Message.LParam;
            if (Tabs.FirstOrDefault(x => x.Windows.Any(y => y == winPtr)) is TabBase Tab)
            {
                Tab.DetachAndDispose(false);
                e.Result = 1;
            }
            else e.Result = 0;
        }
    }

    // Different Thread
    void OnLoopCalled()
    {
        var HasOwner = this.HasOwner;
        if (_HasOwner != HasOwner)
        {
            _HasOwner = HasOwner;
            DispatcherQueue.TryEnqueue(delegate
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasOwner)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SettingsButtonVisibility)));
            });
        }
        {
            static bool IsInTitleBarBounds(WindowEx Main, WindowEx ToCheck)
            {
                var Bounds = Main.Bounds;
                var CursorPos = Cursor.Position;
                if (Bounds.Contains(CursorPos))
                {
                    var foregroundBounds = ToCheck.Bounds;
                    foregroundBounds.Height -= ToCheck.ClientBounds.Height;
                    if (foregroundBounds.Height is <= 16 ||
                        foregroundBounds.Height >> 2 > foregroundBounds.Height) // A >> 2 == A / 2
                        foregroundBounds.Height = 32 * ToCheck.CurrentDisplay.ScaleFactor / 100;
                    if (foregroundBounds.Contains(CursorPos))
                        return true;
                }
                return false;
            }
            static bool IsUnitedSetWindowVisible(WindowEx WindowEx, WindowEx ToCheck)
            {
                if (ToCheck.Bounds.Contains(WindowEx.Bounds))
                    // User can't see United Sets.
                    // User doesn't know United Sets is behind. We can't mess with them.
                    return false;
                foreach (var below in new WindowRelative(ToCheck).GetBelows())
                {
                    Debug.WriteLine(below);
                    var CursorPos = Cursor.Position;
                    if (below == WindowEx)
                        return true;
                    if (below.ClassName is
                        "Qt5152TrayIconMessageWindowClass" or
                        "Qt5152QWindowIcon")
                        continue;
                    if (below.Bounds.Contains(CursorPos))
                        // If there is window above United Sets and it covers up United Sets
                        // Don't add tabs. User can't see the window
                        return false;
                }
                return false;
            }
            Cell? DetectCell()
            {
                var cursorPos = Cursor.Position;
                var windowBounds = WindowEx.Bounds;
                var diffPos = (X: (double)cursorPos.X - windowBounds.X, Y: (double)cursorPos.Y - windowBounds.Y);
                var scale = WindowEx.CurrentDisplay.ScaleFactor / 100d;
                var area = CacheMiddleAreaBounds.Location;
                diffPos = (diffPos.X - area.X * scale, diffPos.Y - area.Y * scale);
                if (diffPos is { X: > 0, Y: > 0 })
                {
                    if (SelectedTabCache is CellTab CellTab)
                    {
                        var normPos = (diffPos.X / windowBounds.Width, diffPos.Y / windowBounds.Height);
                        var info = GetCellAtCursor(normPos, CellTab.MainCell);
                        if (info is not null)
                        {
                            var (rect, cell) = info.Value;
                            return cell;
                        }
                    }
                }
                return null;
            }
            if (Cursor.IsLeftButtonDown && Keyboard.IsControlDown)
            {
                WindowEx OtherWindowDragging = default;
                Cell? SelectedCell = null;
                do
                {
                    var foregroundWindow = WindowEx.ForegroundWindow;
                    if (foregroundWindow != WindowEx)
                    {
                        if (IsInTitleBarBounds(WindowEx, foregroundWindow) && IsUnitedSetWindowVisible(WindowEx, foregroundWindow))
                        {
                            var NewCell = DetectCell();
                            var UpdateHoverToTrue = OtherWindowDragging == default;
                            if (NewCell != SelectedCell)
                                if (SelectedCell is not null)
                                    SelectedCell.HoverEffect = false;
                            if (NewCell is not null)
                                NewCell.HoverEffect = true;
                            if (NewCell is not null)
                            {
                                if (SelectedCell is null && UpdateHoverToTrue == false)
                                    DispatcherQueue.TryEnqueue(() => NoWindowHoveringStoryBoard.Begin());
                            }
                            else if (UpdateHoverToTrue || (SelectedCell is not null && NewCell is null))
                                DispatcherQueue.TryEnqueue(() => WindowHoveringStoryBoard.Begin());
                            SelectedCell = NewCell;
                            OtherWindowDragging = foregroundWindow;
                        }
                        else
                        {
                            if (SelectedCell is not null)
                                SelectedCell.HoverEffect = false;
                            SelectedCell = null;
                            if (OtherWindowDragging != default)
                                DispatcherQueue.TryEnqueue(() => NoWindowHoveringStoryBoard.Begin());
                            OtherWindowDragging = default;
                        }
                    }
                    Thread.Sleep(200);
                } while (Cursor.IsLeftButtonDown);
                if (OtherWindowDragging != default)
                {
                    var window = OtherWindowDragging;
                    OtherWindowDragging = default;
                    DispatcherQueue.TryEnqueue(() => NoWindowHoveringStoryBoard.Begin());
                    var foreground = WindowEx.ForegroundWindow;
                    if (foreground == window &&
                        IsInTitleBarBounds(WindowEx, window) &&
                        IsUnitedSetWindowVisible(WindowEx, window))
                    {
                        if (SelectedCell is not null)
                        {
                            SelectedCell.HoverEffect = false;
                            DispatcherQueue.TryEnqueue(() => SelectedCell.RegisterWindow(window));
                        }
                        else DispatcherQueue.TryEnqueue(() => AddTab(window));
                    }
                }
            }
        }
        //else
        //{
        //    if (OtherWindowDragging != default)
        //    {
        //        DispatcherQueue.TryEnqueue(() => NoWindowHoveringStoryBoard.Begin());
        //        OtherWindowDragging = default;
        //    }
        //}
    }
    static ((double X1, double Y1, double X2, double Y2), Cell)? GetCellAtCursor((double X, double Y) CursorPos, Cell MainCell)
    {
        if (MainCell.HasWindow)
            return null;
        if (MainCell.Empty)
            return ((0, 0, 1, 1), MainCell);
        static (int Index, double RemainingScaled) ComputeScale(int count, double pos)
        {
            // 1 / count * x = value
            // x = value * count
            var idx = (int)(pos * count);
            if (idx == count) idx--;
            // [ pos - (idx / count) ] * count
            // = pos * count - idx
            var remaining = pos * count - idx;
            return (idx, remaining);
        }
        static (double Out1, double Out2) ComputeScaleReversed((double In1, double In2) scaledRect, int idx, int totalCount)
        {
            return (scaledRect.In1 / totalCount + idx / totalCount, scaledRect.In2 / totalCount + idx / totalCount);
        }
        if (MainCell.HasHorizontalSubCells)
        {
            var count = MainCell.SubCells!.Length;
            var (idx, remaining) = ComputeScale(count, CursorPos.X);
            var output = GetCellAtCursor((remaining, CursorPos.Y), MainCell.SubCells[idx]);
            if (output is null) return null;
            var (Rect, cell) = output.Value;
            (Rect.X1, Rect.X2) = ComputeScaleReversed((Rect.X1, Rect.X2), idx, count);
            return (Rect, cell);
        }
        if (MainCell.HasVerticalSubCells)
        {
            var count = MainCell.SubCells!.Length;
            var (idx, remaining) = ComputeScale(count, CursorPos.Y);
            var output = GetCellAtCursor((CursorPos.X, remaining), MainCell.SubCells[idx]);
            if (output is null) return null;
            var (Rect, cell) = output.Value;
            (Rect.Y1, Rect.Y2) = ComputeScaleReversed((Rect.Y1, Rect.Y2), idx, count);
            return (Rect, cell);
        }
        return null;
    }
    async void OnWindowClosing(AppWindow _1, AppWindowClosingEventArgs e)
    {
        e.Cancel = true;
        Dialog.XamlRoot = Content.XamlRoot;
        var item = TabView.SelectedItem;
        TabView.SelectedIndex = -1;
        TabView.Visibility = Visibility.Collapsed;
        WindowEx.Focus();
        ContentDialogResult result;
        try
        {
            result = await Dialog.ShowAsync();
        }
        catch
        {
            result = ContentDialogResult.None;
        }
        switch (result)
        {
            case ContentDialogResult.Primary:
                // Release all windows
                while (Tabs.Count > 0)
                {
                    var Tab = Tabs[0];
                    Tabs.RemoveAt(0);
                    Tab.DetachAndDispose(JumpToCursor: false);
                }
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

    private void FirstRun(object _1, WindowActivatedEventArgs _2)
    {
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
    }

    readonly ContentDialog Dialog = new()
    {
        Title = "Closing UnitedSets",
        Content = "How do you want to close the app?",
        PrimaryButtonText = "Release all Windows",
        SecondaryButtonText = "Close all Windows",
        CloseButtonText = "Cancel"
    };
    readonly AddTabFlyout AddTabFlyout = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private async void AddTab(object _1, RoutedEventArgs e)
    {
        if (Keyboard.IsShiftDown)
        {
            var newTab = new CellTab(this);
            Tabs.Add(newTab);
            TabView.SelectedItem = newTab;
        } else
        {
            WindowEx.Minimize();
            //this.Hide();
            await AddTabFlyout.ShowAtCursorAsync();
            //this.Show();
            WindowEx.Restore();
            var result = AddTabFlyout.Result;
            AddTab(result);
        }
    }
    void AddTab(WindowEx newWindow)
    {
        if (!newWindow.IsValid)
            return;
        newWindow = newWindow.Root;
        if (newWindow.Handle == IntPtr.Zero)
            return;
        if (newWindow.Handle == AddTabFlyout.GetWindowHandle())
            return;
        if (newWindow.Handle == WindowEx.Handle)
            return;
        if (newWindow.ClassName is
            "Shell_TrayWnd" // Taskbar
            or "Progman" // Desktop
            or "WindowsDashboard" // I forget
            or "Windows.UI.Core.CoreWindow" // Quick Settings and Notification Center (other uwp apps should already be ApplicationFrameHost)
            )
            return;
        // Check if United Sets has owner (United Sets in United Sets)
        if (WindowEx.Root.Children.Any(x => x == newWindow))
            return;
        if (Tabs.Any(x => x.Windows.Any(y => y == newWindow)))
            return;
        var newTab = new HwndHostTab(this, newWindow);
        Tabs.Add(newTab);
        TabView.SelectedItem = newTab;
    }

    public static void TabDroppedOutside(TabView _1, TabViewTabDroppedOutsideEventArgs args)
    {
        if (args.Tab.Tag is HwndHostTab Tab)
        {
            Tab.DetachAndDispose(JumpToCursor: true);
        }
    }

    public static void TabDragStarting(TabView _1, TabViewTabDragStartingEventArgs args)
    {
        //var firstItem = args.Tab;
        //args.Data.Properties.Add("UnitedSetsTab", firstItem);
        var item = (HwndHostTab)args.Item;
        var handleInLong = (long)item.Window.Handle.Value;
        var ms = new MemoryStream();
        ms.Write(BitConverter.GetBytes(handleInLong));
        GC.KeepAlive(ms);
        args.Data.SetData("UnitedSetsTabWindow", ms.AsRandomAccessStream());
        //args.Data.RequestedOperation = DataPackageOperation.Move;
    }

    private void TabView_DragOver(object sender, DragEventArgs e)
    {
        if (e.DataView.AvailableFormats.Contains("UnitedSetsTabWindow"))
            e.AcceptedOperation = DataPackageOperation.Move;
        //if (e.Data.Properties["UnitedSetsTab"])
        //PInvoke.SendMessage(WindowEx, UnitedSetCommunicationChangeWindowOwnership, default, new(WindowEx));
    }

    private async void TabView_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.AvailableFormats.Contains("UnitedSetsTabWindow"))
        {
            //var a = Convert.ToInt64(((MemoryStream)await e.DataView.GetDataAsync("UnitedSetsTabWindow")).readz);
            var a = (long)await e.DataView.GetDataAsync("UnitedSetsTabWindow");
            var window = WindowEx.FromWindowHandle((nint)a);
            var ret = PInvoke.SendMessage(window.Owner, UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
            AddTab(window);
        }
    }
}
