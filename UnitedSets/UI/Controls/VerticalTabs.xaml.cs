using Windows.ApplicationModel.DataTransfer;
using UnitedSets.Tabs;

namespace UnitedSets.UI.Controls;

// [DependencyProperty<object>("SelectedItem", UseNullableReferenceType = true)]
[DependencyProperty<object>("Footer", UseNullableReferenceType = true)]
public sealed partial class VerticalTabs : Grid
{
    /// <summary>
    /// Identifies the SelectedItem dependency property.
    /// </summary>
    public static global::Microsoft.UI.Xaml.DependencyProperty SelectedItemProperty { get; } = global::Microsoft.UI.Xaml.DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(global::UnitedSets.UI.Controls.VerticalTabs),
        new global::Microsoft.UI.Xaml.PropertyMetadata(
            default(object),
            static (d, e) => {
                var @this = ((global::UnitedSets.UI.Controls.VerticalTabs)d);
            }
        )
    );

    // No Documentation was provided
    public object? SelectedItem
    {
        // No Documentation was provided
        get
        {
            return (object?)(GetValue(SelectedItemProperty));
        }

        // No Documentation was provided
        set
        {
            if (global::System.Collections.Generic.EqualityComparer<object>.Default.Equals(SelectedItem, value)) return;
            SetValue(SelectedItemProperty, value);
        }
    }
    public VerticalTabs()
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
            ListView.ItemsSource = value?.Tabs;
        }
    }
    MainWindow? _Owner;

    void ListView_TabDragStarting(object sender, DragItemsStartingEventArgs args)
    {
        if (args.Items.Count > 0 && args.Items[0] is TabBase item)
            TabDragDrop.OnDragStarting(item, args.Data);
    }

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
            ListView.SelectedIndex = Owner.Tabs.IndexOf(tb);
    }
    [Event(typeof(DragEventHandler))]
    void OnDropOverTabView(DragEventArgs e)
    {
        if (Owner is null) return;
        var index = TabDragDrop.GetIndexAt(ListView.ContainerFromIndex, Owner.Tabs.Count, this, e.GetPosition(this));
        TabDragDrop.OnDrop(Owner, e, index);
    }

    private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        if (args.DropResult is DataPackageOperation.None && args.Items.Count > 0 && args.Items[0] is TabBase Tab)
            // (the old code checked for a FrameworkElement here, but the items are the tabs themselves,
            // so dragging a vertical tab out never released the window)
            TabDragDrop.OnDroppedOutside(Tab);
        else
            TabDragDrop.OnDragCompleted();
    }

    public event SelectionChangedEventHandler? SelectionChanged;

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => SelectionChanged?.Invoke(sender, e);
}
