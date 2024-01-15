using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using Windows.Foundation;

namespace Get.XAMLTools;
[DependencyProperty<Orientation>("Orientation", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodWithParameter = false, LocalOnPropertyChangedMethodName = nameof(InvalidateArrange), DefaultValueExpression = "global::Microsoft.UI.Xaml.Controls.Orientation.Vertical")]
[AttachedProperty(typeof(double), "RelativeSize", GenerateLocalOnPropertyChangedMethod = true)]
public partial class EzRelativeStackPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }
    (double Adaptive, double Full) ToOrientedSize(Size s)
        => Orientation == Orientation.Vertical ? (s.Height, s.Width) : (s.Width, s.Height);
    Size SizeFromOriented(double adaptive, double full)
        => Orientation == Orientation.Vertical ? new(full, adaptive) : new(adaptive, full);
    Point PointFromOriented(double adaptive, double full)
        => Orientation == Orientation.Vertical ? new(full, adaptive) : new(adaptive, full);
    static partial void OnRelativeSizeChanged(DependencyObject obj, double oldValue, double newValue)
        => (VisualTreeHelper.GetParent(obj) as EzRelativeStackPanel)?.InvalidateArrange();
    protected override Size ArrangeOverride(Size finalSize)
    {
        try
        {
            double Used = 0;
            var childrenAndRS = (from x in Children select (UIElement: x, RelativeSize: GetRelativeSize(x))).ToArray();
            var totalRS = childrenAndRS.Sum(x => x.RelativeSize);
            var ChildrenCount = Children.Count;
            var finalOrientedSize = ToOrientedSize(finalSize);
            var Multipier = finalOrientedSize.Adaptive / totalRS;
            foreach (var (child, RelativeSize) in childrenAndRS)
            {
                var RequestedSize = Multipier * RelativeSize;
                child.Measure(SizeFromOriented(RequestedSize, finalOrientedSize.Full));
                child.Arrange(new(
                        PointFromOriented(Used, 0),
                        SizeFromOriented(RequestedSize, finalOrientedSize.Full))
                    );
                Used += RequestedSize;
            }
            return SizeFromOriented(Math.Max(Used, 0), finalOrientedSize.Full);
        } catch
        {
            return finalSize;
        }
    }
}