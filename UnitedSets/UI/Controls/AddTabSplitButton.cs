using UnitedSets.Apps;
using UnitedSets.Tabs;
using WinWrapper.Input;

namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    private bool ShouldUseSplitButton = true;
    <root @SizeChanged+=`UpdateShouldUseSplitButtion()` @Loaded+=`UpdateShouldUseSplitButtion()`>
        if (`ShouldUseSplitButton`)
            <SplitButton
                Padding=`new(6,2,6,2)` Height=28
                StretchH
                BorderThickness=`new(0)`
                Background=`Solid(Colors.Transparent)`
                HorizontalContentAlignment=Center VerticalContentAlignment=Center
                @Click+=`OnAddTabClick()`
                Flyout = flyoutCopy = <MenuFlyout Placement=BottomEdgeAlignedRight !ShouldConstrainToRootBounds>
                        <MenuFlyoutItem @Click+=`UnitedSetsApp.Current.OpenAddTabDialog(Owner)` Text="Add Window" />
                        <MenuFlyoutItem @Click+=`AddSplitableTab()` Text="Add Splitable Tab" />
                        <MenuFlyoutSeparator />
                        <MenuFlyoutItem @Click+=`UnitedSetsApp.Current.CreateNewSetWindow()` Text="New Set Window" />
                    </MenuFlyout>
            >
                <FluentSymbolIcon Symbol=Add20 Margin=`new(-2,-2,0,0)` Width=16 Height=16 />
            </SplitButton>
        else
            smallBtn = <Button
                Padding=5 StretchH
                Content = <SymbolIcon Symbol=Add />
                @Click+=`OnSimplifiedAddTabClick()`
            />
    </root>
    """)]
partial class AddTabSplitButton : Grid
{
    public AddTabSplitButton()
    {
        Init();
    }
    /// <summary>
    /// The set window new tabs are added to
    /// </summary>
    public MainWindow? Owner { get; set; }
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
            UnitedSetsApp.Current.OpenAddTabDialog(Owner);
        }
    }
    void AddSplitableTab()
    {
        var target = Owner ?? UnitedSetsApp.Current.MainWindow;
        var newTab = new CellTab(Constants.IsAltTabVisible);
        target.Tabs.Add(newTab);
        target.SelectedTab = newTab;
    }
}
