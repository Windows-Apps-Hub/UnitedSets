using Microsoft.UI.Xaml.Controls.Primitives;
using UnitedSets.Tabs;
namespace UnitedSets.UI.Controls.MainArea;

[QuickMarkup("""
    using UnitedSets.Apps;
    using UnitedSets.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;
    GridLength LeftInset;
    GridLength RightInset;
    private bool HasTab => `UnitedSetsApp.Current.SelectedTab is not null`;
    <root StretchH Orientation=Horizontal>
        <UnitedSetsDragRegion Background=`Solid(Colors.Transparent)` OrientedStack_Length=`new GridLength(250)`>
            <.RowDefinitions>
                <RowDefinition Auto MinHeight=32 />
                <RowDefinition />
            </.RowDefinitions>
            <Grid>
                <.ColumnDefinitions>
                    <ColumnDefinition Width=`LeftInset` />
                    <ColumnDefinition Auto />
                    <ColumnDefinition />
                    <ColumnDefinition Auto />
                </.ColumnDefinitions>
                <HStack Left Margin=`new(12, 0, 0, 0)` Grid_Column=1>
                    <Image Width=24 Height=24 Left CenterV
                        Source=`new BitmapImage(new Uri("ms-appx:///Assets/Square44x44Logo.scale-100.png"))`
                    />
                    <HStack Left Spacing=4 Grid_Column=1 IsVisible=`!HasTab`>
                        <TextBlock CenterV Margin=`new(8, 0, 0, 0)`
                            Text="United Sets" 
                            Style=`(Style)App.Current.Resources["CaptionTextBlockStyle"]` />
                        <TextBlock CenterV Opacity=0.7
                            Text=`Constants.AppVersionTag`
                            Style=`(Style)App.Current.Resources["CaptionTextBlockStyle"]` 
                        />
                    </HStack>
                </HStack>
                <HStack CenterV Grid_Column=3>
                    <MainWindowControlButton
                        CenterV
                        Style=`(Style)App.Current.Resources["ToolbarButton"]`
                        MainWindow=`UnitedSetsApp.Current.MainWindow`
                    />
                    <HStack CenterV Grid_Column=3 IsVisible=`HasTab`>
                        minimizeBtn = <Button CenterV BorderThickness=0 Padding=5 Content=<SymbolExIcon SymbolEx=ChromeMinimize /> DragRegion_ClientKind=`NonClientRegionKind.Minimize` />
                        maximizeBtn = <Button CenterV BorderThickness=0 Padding=5 Content=<SymbolExIcon SymbolEx=ChromeMaximize /> DragRegion_ClientKind=`NonClientRegionKind.Maximize` />
                        closeBtn = <Button CenterV BorderThickness=0 Padding=5 Content=<SymbolIcon Symbol=Clear /> DragRegion_ClientKind=`NonClientRegionKind.Close` />
                    </HStack>
                </HStack>
            </Grid>
            <VerticalTabs StretchH StretchV Grid_Row=1 TitleBarInteractable
                SelectedItem=`UnitedSetsApp.Current.SelectedTab`
                SelectedItem=>`SelectedTabBindback`
                @SelectionChanged+=`TabSelectionChanged?.Invoke()`
                Footer=<AddTabSplitButton
                    Canvas_ZIndex=8
                    StretchH
                />
            />
        </UnitedSetsDragRegion>
        <VerticalTabsResizer OrientedStack_Length=`Auto()`
            Right StretchV
            Canvas_ZIndex=99 MinWidth=5 MinHeight = 5
            Minimum=250
            Orientation=`Orientation.Vertical`
            Background=`Solid(Colors.Transparent)`
            IsEnabled
        />
        MainAreaBorder = <Border Grid_Column=1 OrientedStack_Length=`Star()`>
            <TabVisualizer
                Tab=`UnitedSetsApp.Current.SelectedTab`
                HorizontalContentAlignment=Stretch
                VerticalContentAlignment=Stretch
            />
        </Border>
    </root>
    """)]
partial class VerticalTabsFullPanel : OrientedStack, IMainAreaPanel
{
    static object SelectedTabBindback
    {
        set => UnitedSetsApp.Current.SelectedTab = (TabBase)value;
    }
    public event Action? TabSelectionChanged;
    public System.Drawing.Rectangle GetMainAreaRectangle(UIElement reference)
    {
        var Pt = MainAreaBorder.TransformToVisual(reference).TransformPoint(
            new Point(0, 0)
        );
        var size = MainAreaBorder.ActualSize;
        return new System.Drawing.Rectangle((int)Pt._x, (int)Pt._y, (int)size.X, (int)size.Y);
    }
    public void ShowClosingFlyout(FlyoutBase flyoutBase)
    => flyoutBase.ShowAt(closeBtn.IsVisible ? closeBtn : MainAreaBorder);
}
