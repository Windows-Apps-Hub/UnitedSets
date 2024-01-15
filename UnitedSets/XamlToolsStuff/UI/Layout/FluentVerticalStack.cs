using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace Get.XAMLTools;
[DependencyProperty<int>("ElementPadding", DefaultValueExpression = "16", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodWithParameter = false, LocalOnPropertyChangedMethodName = nameof(InvalidateArrange))]
public partial class FluentVerticalStack : Panel
{
    public FluentVerticalStack() { }
    public FluentVerticalStack(int ElementPadding)
    { this.ElementPadding = ElementPadding; }
    protected override Size MeasureOverride(Size availableSize)
    {
        double UsedHeight = 0, UsedWidth = 0;
        foreach (var child in Children)
        {
            if (child.Visibility == Visibility.Collapsed) continue;
            child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            UsedHeight += child.DesiredSize.Height + ElementPadding;
            UsedWidth = Math.Max(UsedWidth, child.DesiredSize.Width);
        }
        UsedHeight -= ElementPadding;
        var size = new Size(
            HorizontalAlignment != HorizontalAlignment.Stretch
            ? UsedWidth : availableSize.Width,
            Math.Max(UsedHeight, 0)
        );
        return size;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        double UsedHeight = 0;
        foreach (var child in Children)
        {
            if (child.Visibility == Visibility.Collapsed) continue;
            child.Measure(new Size(finalSize.Width, double.PositiveInfinity)); // finalSize.Height - UsedHeight
            var width = child.DesiredSize.Width;
            //if (child is Image) System.Diagnostics.Debugger.Break();
            switch ((child as FrameworkElement)?.HorizontalAlignment ?? HorizontalAlignment.Stretch)
            {
                case HorizontalAlignment.Left:
                    child.Arrange(new Rect(0, UsedHeight, width, child.DesiredSize.Height));
                    break;
                case HorizontalAlignment.Center:
                    child.Arrange(new Rect((finalSize.Width - width) / 2, UsedHeight, width, child.DesiredSize.Height));
                    break;
                case HorizontalAlignment.Right:
                    child.Arrange(new Rect(finalSize.Width - width, UsedHeight, width, child.DesiredSize.Height));
                    break;
                case HorizontalAlignment.Stretch:
                    child.Arrange(new Rect(0, UsedHeight, finalSize.Width, child.DesiredSize.Height));
                    break;
            }
            UsedHeight += child.DesiredSize.Height + ElementPadding;
        }
        UsedHeight -= ElementPadding;
        return new Size(finalSize.Width, Math.Max(UsedHeight, 0));
    }
}