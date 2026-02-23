using Microsoft.UI.Xaml.Controls.Primitives;

namespace UnitedSets.UI.Controls.MainArea;

[QuickMarkup("""
    using UnitedSets.Apps;
    GridLength LeftInset;
    GridLength RightInset;
    <root>
        <.RowDefinitions>
            <RowDefinition />
            <RowDefinition />
        </.RowDefinitions>
        TabViewBorder = <Border MinHeight=40 PointerMoved+=`DragRegion_PointerMoved` Background=`Solid(Colors.Transparent)`>
            DragRegion = <DragRegion Background=`Solid(Colors.Transparent)` StretchH StretchV>
                <.ColumnDefinitions>
                    <ColumnDefinition Width=`LeftInset` />
                    <ColumnDefinition />
                    <ColumnDefinition Width=32 />
                    <ColumnDefinition Width=`RightInset` />
                </.ColumnDefinitions>
                TabView = <HorizontalTabs Grid_Column=1 Left CenterV
                    SelectedItem=`UnitedSetsApp.Current.SelectedTab`
                    @SelectionChanged+=`TabSelectionChanged?.Invoke()`
                    TabStripHeader=<MainWindowControlButton
                        CenterV
                        Margin=`new(5,0,0,0)`
                        Style=`(Style)App.Current.Resources["ToolbarButton"]`
                        MainWindow=`UnitedSetsApp.Current.MainWindow`
                    />
                    TabStripFooter=<AddTabSplitButton
                        Canvas_ZIndex=8
                        Margin=`new(0,0,20,0)`
                        Left
                        TitleBarInteractable
                    />
                />
            </DragRegion>
        </Border>
        MainAreaBorder = <Border Grid_Row=1>
            <TabVisualizer
                Tab=`UnitedSetsApp.Current.SelectedTab`
                HorizontalContentAlignment=Stretch
                VerticalContentAlignment=Stretch
            />
        </Border>
    </root>
    """)]
partial class HorizontalTabsPanel : Grid
{
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
    public void ShowClosingFlyout(FlyoutBase flyoutBase)
    => flyoutBase.ShowAt(MainAreaBorder);
}
