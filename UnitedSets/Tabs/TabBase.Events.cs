using Microsoft.UI.Xaml.Controls;
using Get.EasyCSharp;
using Windows.Foundation;
using UnitedSets.Mvvm.Services;

namespace UnitedSets.Tabs;

partial class TabBase
{
    [Event(typeof(TypedEventHandler<TabViewItem, TabViewTabCloseRequestedEventArgs>), Visibility = GeneratorVisibility.Public, Name = "TabCloseRequestedEv")]
    void TabCloseRequested()
    {
		DoShowTab();
        if (Settings.CloseTabBehavior.Value is CloseTabBehaviors.CloseWindow)
            _ = TryCloseAsync();
        else
            DetachAndDispose(JumpToCursor: true);
    }
}
