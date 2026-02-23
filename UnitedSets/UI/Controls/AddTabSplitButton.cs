using UnitedSets.Apps;
using UnitedSets.Tabs;
using WinWrapper.Input;

namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    <root
        Padding=5 Height=30
        HorizontalContentAlignment=Center VerticalContentAlignment=Center
        @Click+=`OnAddTabClick()`
        Flyout =
            <MenuFlyout Placement=BottomEdgeAlignedRight !ShouldConstrainToRootBounds>
                <MenuFlyoutItem @Click+=`UnitedSetsApp.Current.OpenAddTabDialog()` Text="Add Window" />
                <MenuFlyoutItem @Click+=`AddSplitableTab()` Text="Add Splitable Tab" />
            </MenuFlyout>
    >
        <FluentSymbolIcon Symbol=Add20 Margin=`new(-2,-2,0,0)` />
    </root>
    """)]
partial class AddTabSplitButton : SplitButton
{
    public AddTabSplitButton()
    {
        DefaultStyleKey = typeof(SplitButton);
        Init();
    }
    void OnAddTabClick()
    {
        if (Keyboard.IsShiftDown)
        {
            AddSplitableTab();
        }
        else
        {
            UnitedSetsApp.Current.OpenAddTabDialog();
        }
    }
    void AddSplitableTab()
    {
        var newTab = new CellTab(Constants.IsAltTabVisible);
        UnitedSetsApp.Current.Tabs.Add(newTab);
        UnitedSetsApp.Current.SelectedTab = newTab;
    }
}
