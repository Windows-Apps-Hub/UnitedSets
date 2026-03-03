using Windows.ApplicationModel.DataTransfer;
using UnitedSets.Tabs;
using UnitedSets.Apps;

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

    void ListView_TabDragStarting(object sender, DragItemsStartingEventArgs args)
    {
        if (args.Items[0] is WindowHostTab item)
            args.Data.Properties.Add(Constants.UnitedSetsTabWindowDragProperty, (long)item.Window.Handle);
    }

    [Event(typeof(DragEventHandler))]
    void OnDragItemOverTabView(DragEventArgs e)
    {
        if (e.DataView.Properties?.ContainsKey(Constants.UnitedSetsTabWindowDragProperty) == true)
            e.AcceptedOperation = DataPackageOperation.Move;
    }

    [Event(typeof(DragEventHandler))]
    void OnDragOverTabViewItem(object sender)
    {
        if (sender is FrameworkElement tvi && tvi.Tag is TabBase tb)
            ListView.SelectedIndex = UnitedSetsApp.Current.Tabs.IndexOf(tb);
    }
    [Event(typeof(DragEventHandler))]
    void OnDropOverTabView(DragEventArgs e)
    {
        if (e.DataView.Properties.TryGetValue(Constants.UnitedSetsTabWindowDragProperty, out var _a) && _a is long a)
        {

            var window = WindowEx.FromWindowHandle((nint)a);
            var ret = window.Owner.SendMessage(
                Constants.UnitedSetCommunicationChangeWindowOwnership, new(), window);
            var pt = e.GetPosition(this);
            var finalIdx = (
                from index in Enumerable.Range(0, UnitedSetsApp.Current.Tabs.Count)
                let ele = ListView.ContainerFromIndex(index) as UIElement
                let posele = ele.TransformToVisual(this).TransformPoint(default)
                let size = ele.ActualSize
                let IsMoreThanTopLeft = pt.X >= posele.X && pt.Y >= posele.Y
                let IsLessThanBotRigh = pt.X <= posele.X + size.X && pt.Y <= posele.Y + size.Y
                where IsMoreThanTopLeft && IsLessThanBotRigh
                select index
            ).FirstOrDefault();
            if (WindowHostTab.Create(window) is { } tab)
                UnitedSetsApp.Current.Tabs.Insert(finalIdx, tab);
        }
    }

    private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        if (args.DropResult is DataPackageOperation.None)
        {
            if (args.Items[0] is FrameworkElement ele && ele.Tag is TabBase Tab)
                Tab.DetachAndDispose(JumpToCursor: true);
        }
    }

    public event SelectionChangedEventHandler? SelectionChanged;

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => SelectionChanged?.Invoke(sender, e);
}
