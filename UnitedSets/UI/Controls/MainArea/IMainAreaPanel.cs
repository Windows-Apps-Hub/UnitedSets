using Microsoft.UI.Xaml.Controls.Primitives;
namespace UnitedSets.UI.Controls.MainArea;

interface IMainAreaPanel
{
    event Action TabSelectionChanged;
    System.Drawing.Rectangle GetMainAreaRectangle(UIElement reference);
    void ShowClosingFlyout(FlyoutBase flyoutBase);
    GridLength LeftInset { get; set; }
    GridLength RightInset { get; set; }
}
