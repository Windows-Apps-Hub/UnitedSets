using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using WindowRelative = WinWrapper.Windowing.WindowRelative;
using WindowEx = WinWrapper.Windowing.Window;
using Cursor = WinWrapper.Input.Cursor;
using UnitedSets.Classes;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using System.Threading;
using Windows.Foundation;
using UnitedSets.Classes.Tabs;
using CommunityToolkit.WinUI;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using WinWrapper.Taskbar;
using WindowHoster;
using WinWrapper;
using Thread = System.Threading.Thread;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
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
        SelectedTabCache = idx < 0 ? null : (idx >= Tabs.Count ? null : Tabs[idx]);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UIRun(DispatcherQueueHandler action) => DispatcherQueue.TryEnqueue(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task UIRunAsync(Action action)
    {
        DispatcherQueue.TryEnqueue(() => action());
        return Task.CompletedTask;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task UIRemoveFromCollection<T>(Collection<T> collection, T item) => UIRunAsync(() => collection.Remove(item));
    private Icon lastIcon = default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateWindowIcon()
    {
        var icon = SelectedTabCache?.Windows.FirstOrDefault().LargeIcon ?? default;
        if (icon != lastIcon)
        {

            Taskbar.SetOverlayIcon(
                Win32Window,
                SelectedTabCache?.Windows.FirstOrDefault().LargeIcon ?? default,
                SelectedTabCache?.Title ?? ""
            );
            lastIcon = icon;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    async Task RemoveDisposedTab()
    {
        foreach (var Tab in Tabs.CacheEnumerable())
        {
            if (Tab.IsDisposed)
                await UIRemoveFromCollection(Tabs, Tab);
        }
        foreach (var TabGroup in HiddenTabs.CacheEnumerable())
        {
            foreach (var Tab in TabGroup.Tabs.CacheEnumerable())
            {
                if (Tab.IsDisposed)
                    await UIRemoveFromCollection(TabGroup.Tabs, Tab);
            }
            if (TabGroup.Tabs.Count == 0)
                await UIRemoveFromCollection(HiddenTabs, TabGroup);
        }
    }
    static ((double X1, double Y1, double X2, double Y2), Cell)? GetCellAtCursor((double X, double Y) CursorPos, Cell MainCell)
    {
        if (MainCell.ContainsWindow)
            return null;
        if (MainCell.IsEmpty)
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
        if (MainCell.ContainsSubCells && MainCell.Orientation is Orientation.Horizontal)
        {
            var count = MainCell.SubCells!.Length;
            var (idx, remaining) = ComputeScale(count, CursorPos.X);
            var output = GetCellAtCursor((remaining, CursorPos.Y), MainCell.SubCells[idx]);
            if (output is null) return null;
            var (Rect, cell) = output.Value;
            (Rect.X1, Rect.X2) = ComputeScaleReversed((Rect.X1, Rect.X2), idx, count);
            return (Rect, cell);
        }
        if (MainCell.ContainsSubCells && MainCell.Orientation is Orientation.Vertical)
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
    (CellTab? tab, Cell? cell) DetectCell()
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
        Cell? SelectedCell = null;
        CellTab? SelectedCellTab = null;
        do
        {
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
                    var registeredWindow = RegisteredWindow.Register(window);
                    if (registeredWindow is not null)
                        DispatcherQueue.TryEnqueue(() => SelectedCell.RegisterWindow(registeredWindow));
                }
                else DispatcherQueue.TryEnqueue(() => AddTab(window));
            }
        }
    }
}
static partial class Extension
{
    // Make the name explicit
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] CacheEnumerable<T>(this IEnumerable<T> values)
        => values.ToArray();
}