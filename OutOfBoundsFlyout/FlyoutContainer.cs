using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Markup;

namespace OutOfBoundsFlyout;

[ContentProperty(Name = nameof(Flyout))]
public class FlyoutContainer
{
    public FlyoutBase? Flyout { get; set; }
}