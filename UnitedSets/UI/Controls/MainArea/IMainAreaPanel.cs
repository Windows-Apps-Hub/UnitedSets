using Microsoft.UI.Xaml.Controls.Primitives;
namespace UnitedSets.UI.Controls.MainArea;

interface IMainAreaPanel
{
    event Action TabSelectionChanged;
    System.Drawing.Rectangle GetMainAreaRectangle(UIElement reference);
    /// <summary>
    /// The area containing the tabs (in DIPs, relative to <paramref name="reference"/>).
    /// Windows dragged over this area are added as a tab.
    /// </summary>
    System.Drawing.Rectangle GetTabStripRectangle(UIElement reference);
    void ShowClosingFlyout(FlyoutBase flyoutBase);
    GridLength LeftInset { get; set; }
    GridLength RightInset { get; set; }

    static System.Drawing.Rectangle GetRectangle(FrameworkElement element, UIElement reference)
    {
        var pt = element.TransformToVisual(reference).TransformPoint(new Point(0, 0));
        var size = element.ActualSize;
        return new((int)pt.X, (int)pt.Y, (int)size.X, (int)size.Y);
    }
}
