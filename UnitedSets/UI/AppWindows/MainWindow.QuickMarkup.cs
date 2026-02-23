namespace UnitedSets.UI.AppWindows;

[QuickMarkup("""
    using UnitedSets.UI.Controls.MainArea;
    <root>
        RootGrid = <Grid
            Canvas_ZIndex=1
            BorderThickness=3
            Background = MainBackgroundColor = <SolidColorBrush(Colors.Transparent) />
            BorderBrush = MainBorderColor = <SolidColorBrush(Colors.Transparent) />
        >
            /* WindowBorderOnTransparent = <Border
                Visibility="Collapsed"
                Grid.RowSpan="50"
                Grid.ColumnSpan="50"
                Canvas.ZIndex="-5"
                HorizontalAlignment="Stretch"
                VerticalAlignment="Stretch"
                CornerRadius="15, 5, 15, 5"
                Background="{ThemeResource CardBackgroundFillColorDefaultBrush}"
                BorderThickness="3"
                BorderBrush=<LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                    <LinearGradientBrush.GradientStops>
                        <GradientStop Color="{ThemeResource SystemListAccentHighColor}" Offset="1"/>
                        <GradientStop Color="{ThemeResource SystemListAccentLowColor}" Offset="0"/>
                    </LinearGradientBrush.GradientStops>
                </LinearGradientBrush>
            /> */
            HoverIndicator = <WindowHoverIndicatorBackground Grid_RowSpan=2 />
            MainAreaPanel = <HorizontalTabsPanel
                LeftInset=`GridLengthFromPixelInt(AppWindow.TitleBar.LeftInset)`
                RightInset=`GridLengthFromPixelInt(AppWindow.TitleBar.RightInset)`
                TabSelectionChanged+=`TabSelectionChanged`
            />
            UnitedSetsHomeBackground = <HomeBackground Grid_Row=1 />
        </Grid>
    </root>
    """)]
partial class MainWindow
{
    private GridLength GridLengthFromPixelInt(int i) => new(i * Win32Window.CurrentDisplay.ScaleFactor / 100);
}
