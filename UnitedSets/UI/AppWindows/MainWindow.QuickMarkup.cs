using UnitedSets.Mvvm.Services;
using UnitedSets.UI.Controls.MainArea;

namespace UnitedSets.UI.AppWindows;

[QuickMarkup("""
    using UnitedSets.UI.Controls.MainArea;
    GridLength LeftInset;
    GridLength RightInset;
    <root>
        RootGrid = <Grid
            Canvas.ZIndex=1
            BorderThickness=3
            Background = MainBackgroundColor = <SolidColorBrush(`Colors.Transparent`) />
            BorderBrush = MainBorderColor = <SolidColorBrush(`Colors.Transparent`) />
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
            HoverIndicator = <WindowHoverIndicatorBackground Grid.RowSpan=2 />
            `CreateMainAreaPanel()`
            UnitedSetsHomeBackground = <HomeBackground Grid.Row=1 />
        </Grid>
    </root>
    """)]
partial class MainWindow
{
    private GridLength GridLengthFromPixelInt(int i) => new(i * Win32Window.CurrentDisplay.ScaleFactor / 100);
    IMainAreaPanel MainAreaPanel;
    UIElement CreateMainAreaPanel()
    {
        MainAreaPanel = UnitedSetsApp.Current.Settings.Layout.Value switch
        {
            UnitedSetsLayouts.VerticalTabs => new VerticalTabsPanel(),
            UnitedSetsLayouts.VerticalTabsFull => new VerticalTabsFullPanel(),
            _ => new HorizontalTabsPanel()
        };
        QUICKMARKUP_DISPOSABLES.Add(global::QuickMarkup.Infra.ReferenceTracker.RunAndRerunOnReferenceChange(() => {
            return LeftInset;
        }, QUICKMARUP_TEMPVALUE => {
            MainAreaPanel.LeftInset = QUICKMARUP_TEMPVALUE;
        }));
        QUICKMARKUP_DISPOSABLES.Add(global::QuickMarkup.Infra.ReferenceTracker.RunAndRerunOnReferenceChange(() => {
            return RightInset;
        }, QUICKMARUP_TEMPVALUE => {
            MainAreaPanel.RightInset = QUICKMARUP_TEMPVALUE;
        }));
        MainAreaPanel.TabSelectionChanged += TabSelectionChanged;
        return (UIElement)MainAreaPanel;
    }
}
