using WinWrapper.Input;

namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    <root
        Padding=5 Height=30
        HorizontalContentAlignment=Center VerticalContentAlignment=Center
        @Click+=`OnAddTabClick()`
        Flyout =
            <MenuFlyout Placement=BottomEdgeAlignedRight !ShouldConstrainToRootBounds>
                <MenuFlyoutItem @Click+=`OnAddTabClick()` Text="Add Window" />
                <MenuFlyoutItem @Click+=`AddSplitableTab?.Invoke()` Text="Add Splitable Tab" />
            </MenuFlyout>
    >
        <FluentSymbolIcon Symbol=Add20 Margin=`new(-2,-2,0,0)` />
    </root>
    """)]
partial class AddTabSplitButton : SplitButton
{
    public event Action? AddTab;
    public event Action? AddSplitableTab;
    public AddTabSplitButton()
    {
        DefaultStyleKey = typeof(SplitButton);
        Init();
    }
    void OnAddTabClick()
    {
        if (Keyboard.IsShiftDown)
        {
            AddSplitableTab?.Invoke();
        }
        else
        {
            AddTab?.Invoke();
        }
    }
}
