using Windows.ApplicationModel.DataTransfer;
using UnitedSets.Apps;
using UnitedSets.Mvvm.Services;
using UnitedSets.Tabs;

namespace UnitedSets.UI.Controls;

/// <summary>
/// Shared drag and drop logic for the tab strips (horizontal and vertical).
/// Tabs can be dragged between set windows of the same process: the tab object is moved as is,
/// so the hosted window keeps all its settings (crop, borderless...) and splitable tabs can be moved too.
/// </summary>
static class TabDragDrop
{
    /// <summary>
    /// The tab currently being dragged from one of our tab strips.
    /// </summary>
    public static TabBase? DraggedTab { get; private set; }

    public static void OnDragStarting(TabBase tab, DataPackage data)
    {
        DraggedTab = tab;
        data.Properties.Add(Constants.UnitedSetsTabDragProperty, true);
        if (tab is WindowHostTab item)
            // also understood by other United Sets processes
            data.Properties.Add(Constants.UnitedSetsTabWindowDragProperty, (long)item.Window.Handle);
        data.RequestedOperation = DataPackageOperation.Move;
    }

    public static bool CanAccept(DragEventArgs e)
        => e.DataView.Properties?.ContainsKey(Constants.UnitedSetsTabDragProperty) == true ||
           e.DataView.Properties?.ContainsKey(Constants.UnitedSetsTabWindowDragProperty) == true;

    /// <summary>
    /// Handles a drop on the tab strip of <paramref name="target"/>.
    /// </summary>
    /// <param name="index">Where to insert the tab, -1 to append</param>
    public static void OnDrop(MainWindow target, DragEventArgs e, int index)
    {
        if (DraggedTab is { } tab && e.DataView.Properties.ContainsKey(Constants.UnitedSetsTabDragProperty))
        {
            if (tab.ParentWindow == target)
                // reordering inside the same tab strip is handled by the control itself
                return;
            e.AcceptedOperation = DataPackageOperation.Move;
            DraggedTab = null;
            // let the drag operation finish before changing the source collection
            target.DispatcherQueue.TryEnqueue(() => UnitedSetsApp.Current.MoveTab(tab, target, index));
            return;
        }
        // a tab from another United Sets process
        if (e.DataView.Properties.TryGetValue(Constants.UnitedSetsTabWindowDragProperty, out var _a) && _a is long a)
        {
            var window = WindowEx.FromWindowHandle((nint)a);
            if (window.Owner.IsValid && UnitedSetsApp.Current.FindSetWindowOf(window.Owner) is null)
                _ = window.Owner.SendMessage(
                    Constants.UnitedSetCommunicationChangeWindowOwnership, new(), window);
            if (WindowHostTab.Create(window) is { } newTab)
            {
                if (index < 0 || index > target.Tabs.Count) target.Tabs.Add(newTab);
                else target.Tabs.Insert(index, newTab);
                target.SelectedTab = newTab;
            }
        }
    }

    /// <summary>
    /// Called when a tab was dropped somewhere that doesn't accept it (desktop, another app...)
    /// </summary>
    public static void OnDroppedOutside(TabBase tab)
    {
        DraggedTab = null;
        if (tab.ParentWindow is not { } source) return;
        if (UnitedSetsApp.Current.Settings.TabDroppedOutsideBehavior.Value is TabDroppedOutsideBehaviors.NewSetWindow)
        {
            if (source.Tabs.Count <= 1)
                // it's the only tab of its window already: nothing to do
                return;
            var cursor = WinWrapper.Input.Cursor.Position;
            var newWindow = UnitedSetsApp.Current.CreateNewSetWindow();
            var bounds = source.Win32Window.Bounds;
            newWindow.Win32Window.Bounds = new(cursor.X - 100, cursor.Y - 20, bounds.Width, bounds.Height);
            UnitedSetsApp.Current.MoveTab(tab, newWindow);
        }
        else
            tab.DetachAndDispose(JumpToCursor: true);
    }

    public static void OnDragCompleted() => DraggedTab = null;

    /// <summary>
    /// Finds the index of the item container under <paramref name="pt"/>, or -1.
    /// </summary>
    public static int GetIndexAt(Func<int, DependencyObject?> containerFromIndex, int count, UIElement relativeTo, Point pt)
    {
        for (int index = 0; index < count; index++)
        {
            if (containerFromIndex(index) is not UIElement ele) continue;
            var pos = ele.TransformToVisual(relativeTo).TransformPoint(default);
            var size = ele.ActualSize;
            if (pt.X >= pos.X && pt.Y >= pos.Y && pt.X <= pos.X + size.X && pt.Y <= pos.Y + size.Y)
                return index;
        }
        return -1;
    }
}
