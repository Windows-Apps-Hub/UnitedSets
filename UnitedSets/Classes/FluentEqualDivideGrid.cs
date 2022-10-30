using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace UnitedSets;

public class FluentHorizontalEqualDivideGrid : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        double UsedWidth = 0;
        var ChildrenCount = Children.Count;
        var DividedWidth = finalSize.Width / ChildrenCount;
        foreach (var child in Children)
        {
            if (child.Visibility == Visibility.Collapsed) continue;
            child.Measure(new Size(DividedWidth, finalSize.Height)); // finalSize.Height - UsedHeight
            var desiredHeight = child.DesiredSize.Height;
            var desiredWidth = DividedWidth;
            //if (child is Image) System.Diagnostics.Debugger.Break();
            switch ((child as FrameworkElement)?.VerticalAlignment ?? VerticalAlignment.Stretch)
            {
                case VerticalAlignment.Top:
                    child.Arrange(new Rect(UsedWidth, 0, desiredWidth, desiredHeight));
                    break;
                case VerticalAlignment.Center:
                    child.Arrange(new Rect(UsedWidth, (finalSize.Height - desiredHeight) / 2, desiredWidth, desiredHeight));
                    break;
                case VerticalAlignment.Bottom:
                    child.Arrange(new Rect(UsedWidth, finalSize.Height - desiredHeight, desiredWidth, desiredHeight));
                    break;
                case VerticalAlignment.Stretch:
                    child.Arrange(new Rect(UsedWidth, 0, desiredWidth, finalSize.Height));
                    break;
            }
            UsedWidth += desiredWidth;
        }
        return new Size(Math.Max(UsedWidth, 0), finalSize.Height);
    }
}
public class FluentVerticalEqualDivideGrid : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        double UsedHeight = 0;
        var ChildrenCount = Children.Count;
        var DividedHeight = finalSize.Height / ChildrenCount;
        foreach (var child in Children)
        {
            if (child.Visibility == Visibility.Collapsed) continue;
            child.Measure(new Size(finalSize.Width, DividedHeight)); // finalSize.Height - UsedHeight
            var desiredWidth = child.DesiredSize.Width;
            var desiredHeight = DividedHeight;
            //if (child is Image) System.Diagnostics.Debugger.Break();
            switch ((child as FrameworkElement)?.VerticalAlignment ?? VerticalAlignment.Stretch)
            {
                case VerticalAlignment.Top:
                    child.Arrange(new Rect(0, UsedHeight, desiredWidth, desiredHeight));
                    break;
                case VerticalAlignment.Center:
                    child.Arrange(new Rect((finalSize.Width - desiredWidth) / 2, UsedHeight, desiredWidth, desiredHeight));
                    break;
                case VerticalAlignment.Bottom:
                    child.Arrange(new Rect(finalSize.Width - desiredWidth, UsedHeight, desiredWidth, desiredHeight));
                    break;
                case VerticalAlignment.Stretch:
                    child.Arrange(new Rect(0, UsedHeight, finalSize.Width, desiredHeight));
                    break;
            }
            UsedHeight += desiredHeight;
        }
        return new Size(Math.Max(UsedHeight, 0), finalSize.Height);
    }
}