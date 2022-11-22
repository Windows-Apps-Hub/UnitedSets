using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace UnitedSets;

public class FluentHorizontalDynamicSizer : Panel
{
    public int LastMinWidth { get; set; } = 150;
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count != 2) return default;
        Children[0].Measure(new Size(availableSize._width - LastMinWidth, availableSize._height));
        return availableSize with { _height = Children[0].DesiredSize._height };
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count != 2) return finalSize;
        var child0avaliablewidth = finalSize._width - LastMinWidth;
        Children[0].Measure(new Size(child0avaliablewidth, finalSize._height));
        var desiredSize = Children[0].DesiredSize;
        Children[0].Arrange(new Rect(0, 0, desiredSize._width, desiredSize._height));
        if (desiredSize._width - child0avaliablewidth < 0.1f)
            desiredSize._width -= 20;
        Children[1].Arrange(new Rect(desiredSize._width, 0, finalSize._width - desiredSize._width, finalSize._height));
        return finalSize;
    }
}