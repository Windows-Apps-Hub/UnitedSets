using Microsoft.UI.Xaml.Controls.Primitives;
using UnitedSets.Tabs;

namespace UnitedSets.UI.Controls.MainArea;

[QuickMarkup("""
    using UnitedSets.Apps;
    GridLength LeftInset;
    GridLength RightInset;
    <root>
        <.RowDefinitions>
            <RowDefinition Auto />
            <RowDefinition />
        </.RowDefinitions>
        TabViewBorder = <Border Height=36 PointerMoved+=`DragRegion_PointerMoved` Background=`Solid(Colors.Transparent)`>
            DragRegion = <UnitedSetsDragRegion Background=`Solid(Colors.Transparent)` StretchH StretchV>
                <.ColumnDefinitions>
                    <ColumnDefinition Width=`LeftInset` />
                    <ColumnDefinition />
                    <ColumnDefinition Width=48 />
                    <ColumnDefinition Width=`RightInset` />
                </.ColumnDefinitions>
                TabView = <HorizontalTabs Grid.Column=1 Left
                    Owner=`Owner`
                    SelectedItem=`Owner.SelectedTab`
                    SelectedItem=>`SelectedTabBindback`
                    @SelectionChanged+=`TabSelectionChanged?.Invoke()`
                    TabStripHeader=<MainWindowControlButton
                        CenterV Width=30 Height=30
                        Margin=`new(4,0,4,0)`
                        Style=`(Style)App.Current.Resources["ToolbarButton"]`
                        MainWindow=`Owner`
                    />
                    TabStripFooter=<AddTabSplitButton
                        Owner=`Owner`
                        Canvas.ZIndex=8
                        Margin=`new(4,0,0,0)`
                        Left CenterV
                        TitleBarInteractable
                    />
                />
            </UnitedSetsDragRegion>
        </Border>
        MainAreaBorder = <Border Grid.Row=1>
            <TabVisualizer
                Tab=`Owner.SelectedTab`
                HorizontalContentAlignment=Stretch
                VerticalContentAlignment=Stretch
            />
        </Border>
    </root>
    """)]
partial class HorizontalTabsPanel : Grid, IMainAreaPanel
{
    readonly MainWindow Owner;
    public HorizontalTabsPanel(MainWindow owner)
    {
        Owner = owner;
        Init();
    }
    object? SelectedTabBindback
    {
        set => Owner.SelectedTab = value as TabBase;
    }
    public event Action? TabSelectionChanged;
    DateTime LatestUpdate;
    private void DragRegion_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (DateTime.UtcNow - LatestUpdate < TimeSpan.FromSeconds(1))
        {
            LatestUpdate = DateTime.UtcNow;
            return;
        }
        DragRegion.UpdateRegion();
        LatestUpdate = DateTime.UtcNow;
    }
    public System.Drawing.Rectangle GetMainAreaRectangle(UIElement reference)
    {
        var Pt = MainAreaBorder.TransformToVisual(reference).TransformPoint(
            new Point(0, 0)
        );
        var size = MainAreaBorder.ActualSize;
        return new System.Drawing.Rectangle((int)Pt._x, (int)Pt._y, (int)size.X, (int)size.Y);
    }
    public System.Drawing.Rectangle GetTabStripRectangle(UIElement reference)
        => IMainAreaPanel.GetRectangle(TabViewBorder, reference);
    public void ShowClosingFlyout(FlyoutBase flyoutBase)
    => flyoutBase.ShowAt(MainAreaBorder);
}
