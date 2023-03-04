using Microsoft.UI.Xaml.Controls;
using EasyCSharp;
using Windows.Foundation;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    [Event(typeof(TypedEventHandler<TabViewItem, TabViewTabCloseRequestedEventArgs>), Visibility = GeneratorVisibility.Public, Name = "TabCloseRequestedEv")]
    void TabCloseRequested(TabViewItem sender)
    {
        ParentTabView.SelectedItem = sender;
        if (Settings.ExitOnClose)
            _ = TryCloseAsync();
        else
            DetachAndDispose(JumpToCursor: true);
    }
}
