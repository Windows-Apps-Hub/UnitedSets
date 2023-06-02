using Microsoft.UI.Xaml.Controls;
using EasyCSharp;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Input;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    [Event(typeof(TypedEventHandler<TabViewItem, TabViewTabCloseRequestedEventArgs>), Visibility = GeneratorVisibility.Public, Name = "TabCloseRequestedEv")]
    void TabCloseRequested(TabViewItem sender)
    {
		DoShowTab();
        if (Settings.CloseWindowOnCloseTab)
            _ = TryCloseAsync();
        else
            DetachAndDispose(JumpToCursor: true);
    }
}
