using Windows.ApplicationModel.DataTransfer;
using UnitedSets.Tabs;

namespace UnitedSets.UI.Controls;

public sealed partial class HorizontalTabs : TabView
{
    public HorizontalTabs()
    {
        InitializeComponent();
    }

    /// <summary>
    /// The set window whose tabs are shown
    /// </summary>
    public MainWindow? Owner
    {
        get => _Owner;
        set
        {
            _Owner = value;
            TabItemsSource = value?.Tabs;
        }
    }
    MainWindow? _Owner;

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDragStartingEventArgs>))]
    void _TabDragStarting(TabViewTabDragStartingEventArgs args)
    {
        if (args.Item is TabBase item)
            TabDragDrop.OnDragStarting(item, args.Data);
    }

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDroppedOutsideEventArgs>))]
    void _TabDroppedOutside(TabViewTabDroppedOutsideEventArgs args)
    {
        if (args.Item is TabBase Tab)
            TabDragDrop.OnDroppedOutside(Tab);
    }

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDragCompletedEventArgs>))]
    void _TabDragCompleted() => TabDragDrop.OnDragCompleted();

    [Event(typeof(DragEventHandler))]
    void OnDragItemOverTabView(DragEventArgs e)
    {
        if (TabDragDrop.CanAccept(e))
            e.AcceptedOperation = DataPackageOperation.Move;
    }

    [Event(typeof(DragEventHandler))]
    void OnDragOverTabViewItem(object sender)
    {
        // switch to the tab under the cursor (e.g. to drop into a cell of a splitable tab)
        if (Owner is not null && sender is FrameworkElement tvi && tvi.Tag is TabBase tb && TabDragDrop.DraggedTab != tb)
            SelectedIndex = Owner.Tabs.IndexOf(tb);
    }
    [Event(typeof(DragEventHandler))]
    void OnDropOverTabView(DragEventArgs e)
    {
        if (Owner is null) return;
        var index = TabDragDrop.GetIndexAt(ContainerFromIndex, Owner.Tabs.Count, this, e.GetPosition(this));
        TabDragDrop.OnDrop(Owner, e, index);
    }
}
