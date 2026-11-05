using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using UnitedSets.Apps;
using UnitedSets.Cells.Data;
using UnitedSets.Helpers;
using UnitedSets.Mvvm.Services;
using UnitedSets.Tabs;
using WindowHoster;
using WinWrapper.Windowing;
using Cursor = WinWrapper.Input.Cursor;
using Keyboard = WinWrapper.Input.Keyboard;

namespace UnitedSets.Services;

/// <summary>
/// Groupy-style grouping. While dragging a window by its title bar:
/// <list type="bullet">
/// <item>drop it on another (normal) window: both windows become tabs of a new set that takes the place of that window;</item>
/// <item>drop it on a set: it is added as a tab (or into an empty cell of a splitable tab).</item>
/// </list>
/// By default grouping only happens while <c>Shift</c> is held (like Groupy; Ctrl, Alt or Tab can be chosen instead),
/// so moving windows around never groups them by accident. A highlight shows where the window will go.
/// </summary>
/// <remarks>
/// Uses <c>EVENT_SYSTEM_MOVESIZESTART/END</c> to know when the user drags a window, and only polls the cursor
/// while a drag is in progress.
/// </remarks>
static class WindowDragToGroupService
{
    static DispatcherQueueTimer? timer;
    static WindowEx dragged;
    static MainWindow? hoverSet;
    static EmptyCell? hoverCell;
    static DropTarget? lastTarget;
    static DateTime lastTargetTime;

    /// <param name="Set">The set to add the window to, or null to create a new set with <paramref name="Window"/></param>
    /// <param name="Window">The window under the cursor (the set window itself, or the window to group with)</param>
    /// <param name="Cell">The empty cell to put the window in</param>
    /// <param name="Highlight">The area to highlight (screen pixels), if any</param>
    record DropTarget(MainWindow? Set, WindowEx Window, EmptyCell? Cell, System.Drawing.Rectangle? Highlight);

    public static void Start()
    {
        WinEvents.RegisterGlobal(WinEventTypes.WindowMovedStart, OnMoveSizeStart);
        WinEvents.RegisterGlobal(WinEventTypes.WindowMovedEnd, OnMoveSizeEnd);
    }

    static DragToGroupModes Mode => UnitedSetsApp.Current.Settings.DragToGroupMode.Value;

    static void OnMoveSizeStart(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        try
        {
            if (!WinEvents.IsWindowItself(idObject, idChild)) return;
            if (Mode is DragToGroupModes.Disabled) return;
            var window = WindowEx.FromWindowHandle(hwnd);
            if (!IsGroupable(window)) return;
            dragged = window;
            lastTarget = null;
            EnsureTimer();
            timer!.Start();
        }
        catch (Exception ex)
        {
            CrashLog.Write("Drag to group (start)", ex);
        }
    }

    static async void OnMoveSizeEnd(WinEventTypes eventType, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (!WinEvents.IsWindowItself(idObject, idChild)) return;
        if (dragged.Handle != hwnd) return;
        var window = dragged;
        try
        {
            timer?.Stop();
            // evaluate one last time at the release position
            var target = FindDropTarget(window);
            // be forgiving when Shift was released a split second before the mouse button
            if (target is null && lastTarget is not null && DateTime.UtcNow - lastTargetTime < TimeSpan.FromMilliseconds(300))
                target = lastTarget;
            ClearHover();
            dragged = default;
            lastTarget = null;
            if (target is null) return;
            // If the window was a tab, it is being detached because the user dragged it.
            // Give that a moment to finish before adding it again.
            if (UnitedSetsApp.Current.FindTab(window.Handle) is not null)
                await Task.Delay(300);
            if (!window.IsValid) return;
            if (target.Set is null)
                await GroupIntoNewSetAsync(target.Window, window);
            else if (target.Cell is not null)
                await MainWindow.DropWindowIntoCellAsync(target.Cell, window);
            else
                AddToSet(target.Set, window);
        }
        catch (Exception ex)
        {
            CrashLog.Write("Drag to group (end)", ex);
        }
    }

    static void AddToSet(MainWindow set, WindowEx window)
    {
        if (UnitedSetsApp.Current.FindTab(window.Handle) is { } existing)
        {
            // still a tab (moving windows is set to not detach them): just move the tab
            UnitedSetsApp.Current.MoveTab(existing.Tab, set);
            return;
        }
        if (WindowHostTab.Create(window) is { } tab)
        {
            set.Tabs.Add(tab);
            set.SelectedTab = tab;
            set.Activate();
        }
    }

    /// <summary>
    /// Groups two windows: a new set window takes the place (and size) of <paramref name="target"/>,
    /// with <paramref name="target"/> as the first tab and <paramref name="dropped"/> as the second (selected) tab.
    /// </summary>
    static async Task GroupIntoNewSetAsync(WindowEx target, WindowEx dropped)
    {
        if (!target.IsValid) return;
        var bounds = target.Bounds;
        var maximized = target.IsMaximizedReliable();
        var targetTab = WindowHostTab.Create(target, shouldBeHidden: true);
        if (targetTab is null) return;
        var droppedTab = WindowHostTab.Create(dropped);
        if (droppedTab is null)
        {
            await targetTab.DetachAndDisposeAsync();
            return;
        }
        var set = UnitedSetsApp.Current.CreateNewSetWindow(maximized ? null : bounds);
        set.Tabs.Add(targetTab);
        set.Tabs.Add(droppedTab);
        set.SelectedTab = droppedTab;
        if (maximized)
            set.Maximize();
    }

    static void EnsureTimer()
    {
        if (timer is not null) return;
        timer = UnitedSetsApp.Current.DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(30);
        timer.Tick += (_, _) =>
        {
            try
            {
                if (!dragged.IsValid || !Cursor.IsLeftButtonDown)
                {
                    // the drag ended without us noticing (e.g. the window was destroyed)
                    timer.Stop();
                    ClearHover();
                    return;
                }
                var target = FindDropTarget(dragged);
                if (target is not null)
                {
                    lastTarget = target;
                    lastTargetTime = DateTime.UtcNow;
                }
                SetHover(target);
            }
            catch (Exception ex)
            {
                timer.Stop();
                ClearHover();
                CrashLog.Write("Drag to group (tick)", ex);
            }
        };
    }

    static void SetHover(DropTarget? target)
    {
        var set = target?.Set;
        var cell = target?.Cell;
        if (set != hoverSet)
        {
            hoverSet?.SetDropHover(false);
            set?.SetDropHover(true);
            hoverSet = set;
        }
        if (cell != hoverCell)
        {
            hoverCell?.HoverEffect = false;
            cell?.HoverEffect = true;
            hoverCell = cell;
        }
        if (target?.Highlight is { } rect)
            DropHighlightOverlay.Show(rect);
        else
            DropHighlightOverlay.Hide();
    }
    static void ClearHover() => SetHover(null);

    /// <summary>
    /// Whether the window can be grouped (dragged into a set, or used as the target to create a set)
    /// </summary>
    static bool IsGroupable(WindowEx window)
    {
        if (!window.IsValid || !window.IsVisible) return false;
        // only top-level windows
        if (window.Root != window) return false;
        if (UnitedSetsApp.Current.AllUnitedSetsWindows.Contains(window)) return false;
        if (Constants.ShouldBeBlacklisted(window)) return false;
        // floating tool palettes, notifications, etc.
        if (window[WindowExStyles.TOOLWINDOW] || window[WindowExStyles.NOACTIVATE]) return false;
        return true;
    }

    /// <summary>
    /// Finds where the dragged window would go if it was dropped now.
    /// </summary>
    static DropTarget? FindDropTarget(WindowEx dragged)
    {
        var mode = Mode;
        if (mode is DragToGroupModes.Disabled) return null;
        // "shift": the grouping key is held. In "no key" mode, Shift still allows dropping anywhere.
        var shift = IsGroupingKeyDown(mode);
        if (mode is not DragToGroupModes.WithoutKey && !shift) return null;
        var cursor = Cursor.Position;
        if (FindTopmostWindowAt(cursor, dragged) is not var (set, window)) return null;
        if (set is not null)
        {
            var strip = set.GetTabStripScreenRectangle();
            if (shift)
            {
                // anywhere on the set, or into an empty cell
                var (_, cell) = set.DetectCell();
                return new(set, set.Win32Window, cell, cell is null ? strip : null);
            }
            // without a key: on the tab bar, or on the title bar of the tabbed window right below it
            var zone = strip with { Height = strip.Height + TitleBand(set.Win32Window).Height };
            return zone.Contains(cursor) ? new(set, set.Win32Window, null, strip) : null;
        }
        if (!UnitedSetsApp.Current.Settings.GroupWithAnyWindow.Value) return null;
        if (window.Owner.IsValid || !IsGroupable(window)) return null;
        var band = TitleBand(window);
        return shift || band.Contains(cursor) ? new(null, window, null, band) : null;
    }

    static bool IsGroupingKeyDown(DragToGroupModes mode) => mode switch
    {
        DragToGroupModes.HoldCtrl => Keyboard.IsControlDown,
        DragToGroupModes.HoldAlt => Keyboard.IsAltDown,
        DragToGroupModes.HoldTab => (GetAsyncKeyState(0x09 /* VK_TAB */) & 0x8000) != 0,
        _ => Keyboard.IsShiftDown
    };

    /// <summary>
    /// The title bar area of a window (screen pixels): where tabs will be once it's grouped.
    /// </summary>
    static System.Drawing.Rectangle TitleBand(WindowEx window)
    {
        var frame = GetFrameBounds(window.Handle);
        var scale = GetDpiForWindow(window.Handle) / 96d;
        if (scale <= 0) scale = 1;
        var clientOrigin = new POINT();
        ClientToScreen(window.Handle, ref clientOrigin);
        var nonClientTop = clientOrigin.Y - frame.Y;
        // Windows with a custom title bar (Chrome, VS Code, Explorer...) draw it in the client area
        var height = nonClientTop >= 20 * scale ? nonClientTop : (int)(40 * scale);
        height = Math.Clamp(height, (int)(24 * scale), (int)(64 * scale));
        return frame with { Height = Math.Min(height, frame.Height) };
    }

    /// <summary>
    /// Walks the windows from top to bottom (z-order) and returns the first one under <paramref name="point"/>,
    /// ignoring the window being dragged. If it is (or is hosted by) a set window, that set is returned too.
    /// </summary>
    static (MainWindow? Set, WindowEx Window)? FindTopmostWindowAt(System.Drawing.Point point, WindowEx dragged)
    {
        var setWindows = UnitedSetsApp.Current.SetWindows.ToArray();
        (MainWindow?, WindowEx)? result = null;
        EnumWindows((hwnd, _) =>
        {
            if (hwnd == DropHighlightOverlay.Handle) return true;
            var w = WindowEx.FromWindowHandle(hwnd);
            if (w == dragged || w.Owner == dragged) return true;
            if (!w.IsVisible || IsIconic(hwnd) || IsCloaked(hwnd)) return true;
            if (!GetFrameBounds(hwnd).Contains(point)) return true;
            // click-through overlays (game bars, screen recorders...) don't count
            if (w[WindowExStyles.Transparent]) return true;
            // a set window, or a window hosted by one
            var set = setWindows.FirstOrDefault(s => s.Win32Window == w || (w.Owner.IsValid && s.Win32Window == w.Owner))
                ?? UnitedSetsApp.Current.FindTab(hwnd)?.Window;
            result = (set, w);
            return false;
        }, 0);
        return result;
    }

    /// <summary>
    /// The visible bounds of a window (without the invisible resize borders)
    /// </summary>
    static System.Drawing.Rectangle GetFrameBounds(nint hwnd)
    {
        if (DwmGetWindowAttribute(hwnd, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT r, Marshal.SizeOf<RECT>()) != 0)
            GetWindowRect(hwnd, out r);
        return new(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
    }

    static bool IsCloaked(nint hwnd)
        => DwmGetWindowAttribute(hwnd, DWMWA_CLOAKED, out int cloaked, sizeof(int)) == 0 && cloaked != 0;

    const int DWMWA_CLOAKED = 14, DWMWA_EXTENDED_FRAME_BOUNDS = 9;
    [StructLayout(LayoutKind.Sequential)]
    struct RECT { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)]
    struct POINT { public int X, Y; }
    delegate bool EnumWindowsProc(nint hwnd, nint lParam);
    [DllImport("user32.dll")]
    static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);
    [DllImport("user32.dll")]
    static extern bool IsIconic(nint hwnd);
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(nint hwnd, out RECT rect);
    [DllImport("user32.dll")]
    static extern bool ClientToScreen(nint hwnd, ref POINT point);
    [DllImport("user32.dll")]
    static extern uint GetDpiForWindow(nint hwnd);
    [DllImport("dwmapi.dll")]
    static extern int DwmGetWindowAttribute(nint hwnd, int attribute, out int value, int size);
    [DllImport("dwmapi.dll")]
    static extern int DwmGetWindowAttribute(nint hwnd, int attribute, out RECT value, int size);
}
