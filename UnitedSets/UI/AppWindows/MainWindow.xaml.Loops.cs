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
using UnitedSets.Mvvm.Services;
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
using UnitedSets.Classes.Tabs;
using Windows.UI.Core;
using CommunityToolkit.WinUI;

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
{
    TabBase? SelectedTabCache;
    public System.Drawing.Rectangle CacheMiddleAreaBounds { get; set; }

    // UI Thread
    [Event(typeof(TypedEventHandler<DispatcherQueueTimer, object>))]
    void OnTimerLoopTick()
    {
        var Pt = MainAreaBorder.TransformToVisual(Content).TransformPoint(
            new Point(0, 0)
        );
        var size = MainAreaBorder.ActualSize;
        CacheMiddleAreaBounds = new System.Drawing.Rectangle((int)Pt._x, (int)Pt._y, (int)size.X, (int)size.Y);
        var idx = TabView.SelectedIndex;
        SelectedTabCache = idx < 0 ? null : (idx >= Tabs.Count ? null : Tabs[idx]);
        if (double.IsNaN(TabViewSizer.Width) && TabViewSizer.ActualWidth != 0)
            TabViewSizer.Width = TabViewSizer.ActualWidth - 1;
        else TabViewSizer.Width = double.NaN;
		
    }

	private Task UIRunAsync(Action action) => DispatcherQueue.EnqueueAsync(action);
	private Task UIRemoveFromCollection<T>(Collection<T> collection, T item) => UIRunAsync(()=>collection.Remove(item));
    // Different Thread
	async void OnLoopCalled()
    {
        WindowEx.SetOverlayIconPtr(new(SelectedTabCache?.Windows.FirstOrDefault().LargeIconPtr ?? (nint)0), SelectedTabCache?.Title ?? "");

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
        foreach (var Tab in Tabs.ToArray())
        {
            if (Tab.IsDisposed)
				await UIRemoveFromCollection(Tabs, Tab);
            }
        foreach (var TabGroup in HiddenTabs.ToArray())
        {
            foreach (var Tab in TabGroup.Tabs.ToArray())
            {
                if (Tab.IsDisposed)
					await UIRemoveFromCollection(TabGroup.Tabs, Tab);
            }
            if (TabGroup.Tabs.Count == 0)
				await UIRemoveFromCollection(HiddenTabs, TabGroup);
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
            (CellTab? tab, Cell ? cell) DetectCell()
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
                            return (CellTab, cell);
                        }
                    }
                }
                return (null,null);
            }
            if (Cursor.IsLeftButtonDown && Keyboard.IsControlDown)
            {
                WindowEx OtherWindowDragging = default;
                Cell? SelectedCell = null;
				CellTab? SelectedCellTab = null;
                do
                {
                    var foregroundWindow = WindowEx.ForegroundWindow;
                    if (foregroundWindow != WindowEx)
                    {
                        if (IsInTitleBarBounds(WindowEx, foregroundWindow) && IsUnitedSetWindowVisible(WindowEx, foregroundWindow))
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
                        IsInTitleBarBounds(WindowEx, window) &&
                        IsUnitedSetWindowVisible(WindowEx, window))
                    {
                        if (SelectedCell is not null)
                        {
                            SelectedCell.HoverEffect = false;
                            DispatcherQueue.TryEnqueue(() => SelectedCell.RegisterWindow(new OurHwndHost(SelectedCellTab!,this, window)));
                        }
                        else DispatcherQueue.TryEnqueue(() => AddTab(window));
                    }
                }
            }
        }
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
}
