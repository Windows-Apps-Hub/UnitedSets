using Microsoft.UI.Xaml.Controls.Primitives;
using UnitedSets.Tabs;
namespace UnitedSets.UI.Controls.MainArea;

[QuickMarkup("""
    using UnitedSets.Apps;
    using UnitedSets.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;
    GridLength LeftInset;
    GridLength RightInset;
    private bool HasTab => `Owner.SelectedTab is not null`;
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
                <StackPanel Orientation=Horizontal Left Margin=`new(12, 0, 0, 0)` Grid.Column=1>
                    <Image Width=24 Height=24 Left CenterV
                        Source=`new BitmapImage(new Uri("ms-appx:///Assets/Square44x44Logo.scale-100.png"))`
                    />
                    if (`!HasTab`)
                        <StackPanel Orientation=Horizontal Spacing=4>
                            <TextBlock CenterV Margin=`new(8, 0, 0, 0)`
                                Text="United Sets X" 
                                Style=`(Style)App.Current.Resources["CaptionTextBlockStyle"]` />
                            <TextBlock CenterV Opacity=0.7
                                Text=`Constants.AppVersionTag`
                                Style=`(Style)App.Current.Resources["CaptionTextBlockStyle"]` 
                            />
                        </StackPanel>
                </StackPanel>
                <StackPanel Orientation=Horizontal CenterV Grid.Column=3>
                    <MainWindowControlButton
                        CenterV
                        Style=`(Style)App.Current.Resources["ToolbarButton"]`
                        MainWindow=`Owner`
                    />
                    if (`HasTab`)
                    {
                        <Button
                            CenterV BorderThickness=0 Padding=5
                            Content=<SymbolExIcon SymbolEx=ChromeMinimize />
                            `DragRegion_ClientKind`=`NonClientRegionKind.Minimize`
                        />
                        <Button
                            CenterV BorderThickness=0 Padding=5
                            Content=<SymbolExIcon SymbolEx=ChromeMaximize />
                            `DragRegion_ClientKind`=`NonClientRegionKind.Maximize`
                        />
                        closeBtn = <Button
                            CenterV BorderThickness=0 Padding=5
                            Content=<SymbolIcon Symbol=Clear />
                            `DragRegion_ClientKind`=`NonClientRegionKind.Close`
                        />
                    }
                </StackPanel>
            </Grid>
            TabList = <VerticalTabs StretchH StretchV Grid.Row=1 TitleBarInteractable
                Owner=`Owner`
                SelectedItem=`Owner.SelectedTab`
                SelectedItem=>`SelectedTabBindback`
                @SelectionChanged+=`TabSelectionChanged?.Invoke()`
                Footer=<AddTabSplitButton
                    Owner=`Owner`
                    Canvas.ZIndex=8
                    StretchH
                />
            />
        </UnitedSetsDragRegion>
        <VerticalTabsResizer `OrientedStack_Length`=`Auto()`
            Right StretchV
            Canvas.ZIndex=99 MinWidth=5 MinHeight = 5
            Minimum=250
            Orientation=`Orientation.Vertical`
            Background=`Solid(Colors.Transparent)`
            IsEnabled
        />
        MainAreaBorder = <Border Grid.Column=1 `OrientedStack_Length`=`Star()`>
            <TabVisualizer
                Tab=`Owner.SelectedTab`
                HorizontalContentAlignment=Stretch
                VerticalContentAlignment=Stretch
            />
        </Border>
    </root>
    """)]
partial class VerticalTabsFullPanel : OrientedStack, IMainAreaPanel
{
    readonly MainWindow Owner;
    public VerticalTabsFullPanel(MainWindow owner)
    {
        Owner = owner;
        Init();
    }
    object? SelectedTabBindback
    {
        set => Owner.SelectedTab = value as TabBase;
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
    public System.Drawing.Rectangle GetTabStripRectangle(UIElement reference)
        => IMainAreaPanel.GetRectangle(TabList, reference);
    public void ShowClosingFlyout(FlyoutBase flyoutBase)
    => flyoutBase.ShowAt(HasTab && closeBtn is not null ? closeBtn : MainAreaBorder);
}
