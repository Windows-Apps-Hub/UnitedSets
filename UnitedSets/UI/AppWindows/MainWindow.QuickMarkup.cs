using UnitedSets.Mvvm.Services;
using UnitedSets.UI.Controls.MainArea;

namespace UnitedSets.UI.AppWindows;

[QuickMarkup("""
    using UnitedSets.UI.Controls.MainArea;
    using UnitedSets.Tabs;
    GridLength LeftInset;
    GridLength RightInset;
    TabBase? SelectedTab;
    <root>
        RootGrid = <Grid
            Canvas.ZIndex=1
            BorderThickness=0
            Background = MainBackgroundColor = <SolidColorBrush(`Colors.Transparent`) />
            BorderBrush = MainBorderColor = <SolidColorBrush(`Colors.Transparent`) />
        >
            HoverIndicator = <WindowHoverIndicatorBackground Grid.RowSpan=2 />
            `CreateMainAreaPanel()`
            UnitedSetsHomeBackground = <HomeBackground Margin=`new(0, 48, 0, 0)` />
        </Grid>
    </root>
    """)]
partial class MainWindow
{
    private GridLength GridLengthFromPixelInt(int i) => new(i * Win32Window.CurrentDisplay.ScaleFactor / 100);
    IMainAreaPanel MainAreaPanel = null!;
    UIElement CreateMainAreaPanel()
    {
        MainAreaPanel = UnitedSetsApp.Current.Settings.Layout.Value switch
        {
            UnitedSetsLayouts.VerticalTabs => new VerticalTabsPanel(this),
            UnitedSetsLayouts.VerticalTabsFull => new VerticalTabsFullPanel(this),
            _ => new HorizontalTabsPanel(this)
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

    /// <summary>
    /// Highlights the window while another window is dragged over it (drop target feedback)
    /// </summary>
    internal void SetDropHover(bool hovering)
    {
        if (hovering) HoverIndicator.TransitionToHoverState();
        else HoverIndicator.TransitionToNotHoverState();
    }

    /// <summary>
    /// Gets the screen rectangle (physical pixels) of the tab strip, which is where windows can be dropped
    /// to be added as a tab. Must be called on the UI thread.
    /// </summary>
    internal System.Drawing.Rectangle GetTabStripScreenRectangle()
        => ToScreen(MainAreaPanel.GetTabStripRectangle((UIElement)Content));

    /// <summary>
    /// Converts a rectangle in DIPs relative to the window content into screen pixels.
    /// </summary>
    internal System.Drawing.Rectangle ToScreen(System.Drawing.Rectangle dipRect)
    {
        var scale = Content?.XamlRoot?.RasterizationScale ?? Win32Window.CurrentDisplay.ScaleFactor / 100d;
        var origin = new System.Drawing.Point();
        Windows.Win32.PInvoke.ClientToScreen(new(Win32Window.Handle), ref origin);
        return new(
            origin.X + (int)Math.Round(dipRect.X * scale),
            origin.Y + (int)Math.Round(dipRect.Y * scale),
            (int)Math.Round(dipRect.Width * scale),
            (int)Math.Round(dipRect.Height * scale)
        );
    }
}
