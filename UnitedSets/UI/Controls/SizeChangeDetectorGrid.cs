using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace UnitedSets.UI.Controls;
public partial class SizeChangedDetectorPanel : UserControl
{
    public event Action<Size>? SizeUpdated;
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Content is null) return base.ArrangeOverride(finalSize);
        Content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        SizeUpdated?.Invoke(Content.DesiredSize);
        Content.Arrange(new(default, Content.DesiredSize));
        return Content.DesiredSize;
    }
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Content is null) return base.MeasureOverride(availableSize);
        Content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        SizeUpdated?.Invoke(Content.DesiredSize);
        return Content.DesiredSize;
    }
}