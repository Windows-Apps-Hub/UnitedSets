using Microsoft.UI.Windowing;
using Windows.ApplicationModel.DataTransfer;
using Keyboard = WinWrapper.Input.Keyboard;
using WinUIEx.Messaging;
using UnitedSets.Tabs;
using UnitedSets.UI.Popups;
using CommunityToolkit.Mvvm.Input;
using WinWrapper.Windowing;
using UnitedSets.Cells;
using UnitedSets.PostProcessing;
using UnitedSets.Apps;

namespace UnitedSets.UI.Controls;

public sealed partial class HorizontalTabs : TabView
{
    public HorizontalTabs()
    {
        InitializeComponent();
    }

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDragStartingEventArgs>))]
    void _TabDragStarting(TabViewTabDragStartingEventArgs args)
    {
        if (args.Item is WindowHostTab item)
            args.Data.Properties.Add(Constants.UnitedSetsTabWindowDragProperty, (long)item.Window.Handle);
    }

    [Event(typeof(TypedEventHandler<TabView, TabViewTabDroppedOutsideEventArgs>))]
    void _TabDroppedOutside(TabViewTabDroppedOutsideEventArgs args)
    {
        if (args.Tab.Tag is TabBase Tab)
            Tab.DetachAndDispose(JumpToCursor: true);
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
            SelectedIndex = UnitedSetsApp.Current.Tabs.IndexOf(tb);
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
                let ele = ContainerFromIndex(index) as UIElement
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
}
