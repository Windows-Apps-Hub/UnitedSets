using UnitedSets.Mvvm.Services;

namespace UnitedSets.Tabs;

partial class TabBase
{
    [Event(typeof(TypedEventHandler<TabViewItem, TabViewTabCloseRequestedEventArgs>), Visibility = GeneratorVisibility.Public, Name = "TabCloseRequestedEv")]
    [Event(typeof(RoutedEventHandler), Visibility = GeneratorVisibility.Public, Name = "TabCloseClickEv")]
    void TabCloseRequested()
    {
		DoShowTab();
        if (Settings.CloseTabBehavior.Value is CloseTabBehaviors.CloseWindow)
            _ = TryCloseAsync();
        else
            DetachAndDispose(JumpToCursor: true);
    }
}
