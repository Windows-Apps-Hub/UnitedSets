using UnitedSets.Apps;
using UnitedSets.Tabs;
using WinWrapper.Input;

namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    private bool ShouldUseSplitButton = true;
    <root @SizeChanged+=`UpdateShouldUseSplitButtion()` @Loaded+=`UpdateShouldUseSplitButtion()`>
        <SplitButton
            IsVisible=`ShouldUseSplitButton`
            Padding=5 Height=30
            StretchH
            HorizontalContentAlignment=Center VerticalContentAlignment=Center
            @Click+=`OnAddTabClick()`
            Flyout = flyoutCopy = <MenuFlyout Placement=BottomEdgeAlignedRight !ShouldConstrainToRootBounds>
                    <MenuFlyoutItem @Click+=`UnitedSetsApp.Current.OpenAddTabDialog()` Text="Add Window" />
                    <MenuFlyoutItem @Click+=`AddSplitableTab()` Text="Add Splitable Tab" />
                </MenuFlyout>
        >
            <FluentSymbolIcon Symbol=Add20 Margin=`new(-2,-2,0,0)` />
        </SplitButton>
        smallBtn = <Button
            Padding=5 StretchH
            IsVisible=`!ShouldUseSplitButton`
            Content = <SymbolIcon Symbol=Add />
            @Click+=`OnSimplifiedAddTabClick()`
        />
    </root>
    """)]
partial class AddTabSplitButton : Grid
{
    void UpdateShouldUseSplitButtion()
    {
        ShouldUseSplitButton = ActualWidth >= 70;
    }
    void OnSimplifiedAddTabClick()
    {
        if (Keyboard.IsShiftDown)
        {
            AddSplitableTab();
        }
        else
        {
            flyoutCopy.ShowAt(smallBtn);
        }
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
