using Microsoft.UI.Xaml.Controls;
using WindowRelative = WinWrapper.Windowing.WindowRelative;
using WindowEx = WinWrapper.Windowing.Window;
using Cursor = WinWrapper.Input.Cursor;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Windows.Foundation;
using UnitedSets.Tabs;
using System.Runtime.CompilerServices;
using WindowHoster;
using Thread = System.Threading.Thread;
using UnitedSets.Cells;
using UnitedSets.PostProcessing;
using WinWrapper.Input;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    TabBase? SelectedTabCache;
    System.Drawing.Rectangle CacheMiddleAreaBounds;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CacheValue()
    {
        var Pt = MainAreaBorder.TransformToVisual(Content).TransformPoint(
            new Point(0, 0)
        );
        var size = MainAreaBorder.ActualSize;
        CacheMiddleAreaBounds = new System.Drawing.Rectangle((int)Pt._x, (int)Pt._y, (int)size.X, (int)size.Y);
        var idx = TabView.SelectedIndex;
        SelectedTabCache = idx < 0 ? null : (idx >= UnitedSetsApp.Current.Tabs.Count ? null : UnitedSetsApp.Current.Tabs[idx]);
    }

    static ((double X1, double Y1, double X2, double Y2), EmptyCell)? GetCellAtCursor((double X, double Y) CursorPos, Cell MainCell)
    {
        if (MainCell is WindowCell)
            return null;
        if (MainCell is EmptyCell ec)
            return ((0, 0, 1, 1), ec);
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
        if (MainCell is ContainerCell cc)
        {
            if (cc.Orientation is Orientation.Horizontal)
            {
                var count = cc.SubCells.Count;
                var (idx, remaining) = ComputeScale(count, CursorPos.X);
                var output = GetCellAtCursor((remaining, CursorPos.Y), cc.SubCells[idx]);
                if (output is null) return null;
                var (Rect, cell) = output.Value;
                (Rect.X1, Rect.X2) = ComputeScaleReversed((Rect.X1, Rect.X2), idx, count);
                return (Rect, cell);
            }
            if (cc.Orientation is Orientation.Vertical)
            {
                var count = cc.SubCells.Count;
                var (idx, remaining) = ComputeScale(count, CursorPos.Y);
                var output = GetCellAtCursor((CursorPos.X, remaining), cc.SubCells[idx]);
                if (output is null) return null;
                var (Rect, cell) = output.Value;
                (Rect.Y1, Rect.Y2) = ComputeScaleReversed((Rect.Y1, Rect.Y2), idx, count);
                return (Rect, cell);
            }
        }
        return null;
    }
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
    static bool IsUnitedSetWindowVisible(WindowEx Main, WindowEx ToCheck)
    {
        if (ToCheck.Bounds.Contains(Main.Bounds))
            // User can't see United Sets.
            // User doesn't know United Sets is behind. We can't mess with them.
            return false;
        foreach (var below in new WindowRelative(ToCheck).GetBelows())
        {
            Debug.WriteLine(below);
            var CursorPos = Cursor.Position;
            if (below == Main)
                return true;
            if (below.Class.Name is
                "Qt5152TrayIconMessageWindowClass" or
                "Qt5152QWindowIcon")
                continue;
            if (!below.IsVisible) continue;
            if (below.Bounds.Contains(CursorPos))
                // Also Check Region
                if (below.Region is not System.Drawing.Rectangle rect ||
                    new System.Drawing.Rectangle(below.Bounds.X + rect.X, below.Bounds.Y + rect.Y,
                    rect.Width, rect.Height).Contains(CursorPos))
                    // If there is window above United Sets and it covers up United Sets
                    // Don't add tabs. User can't see the window
                    return false;
        }
        return false;
    }
    (CellTab? tab, EmptyCell? cell) DetectCell()
    {
        var cursorPos = Cursor.Position;
        var windowBounds = Win32Window.Bounds;
        var diffPos = (X: (double)cursorPos.X - windowBounds.X, Y: (double)cursorPos.Y - windowBounds.Y);
        var scale = Win32Window.CurrentDisplay.ScaleFactor / 100d;
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
                    return (CellTab, cell);
                }
            }
        }
        return (null, null);
    }
    void WindowDragLogic()
    {
        WindowEx OtherWindowDragging = default;
        EmptyCell? SelectedCell = null;
        CellTab? SelectedCellTab = null;
        do
        {
            if (!Keyboard.IsControlDown)
                // let's no longer allow window dragging into
                // United Sets after control button is released
                return;
            var foregroundWindow = WindowEx.ForegroundWindow;
            if (foregroundWindow != Win32Window)
            {
                if (IsInTitleBarBounds(Win32Window, foregroundWindow) && IsUnitedSetWindowVisible(Win32Window, foregroundWindow))
                {
                    var (NewTab, NewCell) = DetectCell();
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
                    SelectedCellTab = NewTab;
                    OtherWindowDragging = foregroundWindow;
                }
                else
                {
                    if (SelectedCell is not null)
                        SelectedCell.HoverEffect = false;
                    SelectedCell = null;
                    SelectedCellTab = null;
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
                IsInTitleBarBounds(Win32Window, window) &&
                IsUnitedSetWindowVisible(Win32Window, window))
            {
                if (SelectedCell is not null)
                {
                    SelectedCell.HoverEffect = false;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        var registeredWindow = PostProcessingRegisteredWindow.Register(window);
                        if (registeredWindow is not null)
                            DispatcherQueue.TryEnqueue(() => SelectedCell.RegisterWindow(registeredWindow));
                    });
                }
                else DispatcherQueue.TryEnqueue(delegate
                {
                    if (WindowHostTab.Create(window) is { } tab)
                    {
                        UnitedSetsApp.Current.Tabs.Add(tab);
                        UnitedSetsApp.Current.SelectedTab = tab;
                    }
                });
            }
        }
    }
}
