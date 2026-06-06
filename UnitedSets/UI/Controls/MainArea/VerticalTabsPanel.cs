using Microsoft.UI.Xaml.Controls.Primitives;
using UnitedSets.Tabs;
namespace UnitedSets.UI.Controls.MainArea;

[QuickMarkup("""
    using UnitedSets.Apps;
    using UnitedSets.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;
    GridLength LeftInset;
    GridLength RightInset;
    <root>
        <.RowDefinitions>
            <RowDefinition Auto />
            <RowDefinition />
        </.RowDefinitions>
        <UnitedSetsDragRegion Background=`Solid(Colors.Transparent)` StretchH StretchV Height=32>
            <.ColumnDefinitions>
                <ColumnDefinition Width=`LeftInset` />
                <ColumnDefinition Auto />
                <ColumnDefinition />
                <ColumnDefinition Auto />
                <ColumnDefinition Width=`RightInset` />
            </.ColumnDefinitions>
            <HStack Left Margin=`new(12, 0, 0, 0)` Grid.Column=1>
                <Image Width=24 Height=24 Left CenterV
                    Source=`new BitmapImage(new Uri("ms-appx:///Assets/Square44x44Logo.scale-100.png"))`
                />
                <TextBlock CenterV Margin=`new(8, 0, 0, 0)`
                    Text="United Sets" 
                    Style=`(Style)App.Current.Resources["CaptionTextBlockStyle"]` />
                <TextBlock CenterV Opacity=0.7
                    Text=`Constants.AppVersionTag`
                    Style=`(Style)App.Current.Resources["CaptionTextBlockStyle"]` 
                />
            </HStack>
            <HStack CenterV Grid.Column=3>
                <MainWindowControlButton
                    CenterV
                    Style=`(Style)App.Current.Resources["ToolbarButton"]`
                    MainWindow=`UnitedSetsApp.Current.MainWindow`
                />
            </HStack>
        </UnitedSetsDragRegion>
        <OrientedStack Grid.Row=1 Orientation=Horizontal StretchH>
            <VerticalTabs StretchH StretchV OrientedStack_Length=`new GridLength(250)`
                SelectedItem=`UnitedSetsApp.Current.SelectedTab`
                SelectedItem=>`SelectedTabBindback`
                @SelectionChanged+=`TabSelectionChanged?.Invoke()`
                Footer=<AddTabSplitButton
                    Canvas.ZIndex=8
                    StretchH
                    TitleBarInteractable
                />
            />
            <VerticalTabsResizer OrientedStack_Length=`Auto()`
                Right StretchV
                Canvas.ZIndex=99 MinWidth=5 MinHeight = 5
                Orientation=`Orientation.Vertical`
                Background=`Solid(Colors.Transparent)`
                IsEnabled
                TitleBarInteractable
            />
            MainAreaBorder = <Border Grid.Column=1 OrientedStack_Length=`Star()`>
                <TabVisualizer
                    Tab=`UnitedSetsApp.Current.SelectedTab`
                    HorizontalContentAlignment=Stretch
                    VerticalContentAlignment=Stretch
                />
            </Border>
        </OrientedStack>
    </root>
    """)]
partial class VerticalTabsPanel : Grid, IMainAreaPanel
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
    => flyoutBase.ShowAt(MainAreaBorder);
}
