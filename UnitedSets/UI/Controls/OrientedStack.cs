using System.Diagnostics;
using Get.Data.XACL;

namespace Get.UI.Data;

public partial class OrientedStack : NamedPanel
{
    private readonly Dictionary<UIElement, IProperty<Visibility>> CachedChildrenVisibility = new Dictionary<UIElement, IProperty<Visibility>>();

    private readonly HashSet<UIElement> CachedChildren = new HashSet<UIElement>();

    private static readonly IPropertyDefinition<UIElement, Visibility> VisibilityPropertyDefinition;

    public static AttachedProperty<DependencyObject, GridUnitType> LengthTypeProperty { get; }

    public static AttachedProperty<DependencyObject, double> LengthValueProperty { get; }

    public static AttachedProperty<DependencyObject, GridLength> LengthProperty { get; }

    public static AttachedProperty<DependencyObject, bool> VisibilityTrackingProperty { get; }

    public IProperty<Orientation> OrientationProperty { get; } = AutoTyper.Auto(Orientation.Vertical);

    public IProperty<double> SpacingProperty { get; } = AutoTyper.Auto(0.0);

    public Orientation Orientation
    {
        get
        {
            return OrientationProperty.CurrentValue;
        }
        set
        {
            if (!EqualityComparer<Orientation>.Default.Equals(OrientationProperty.CurrentValue, value))
            {
                OrientationProperty.CurrentValue = value;
            }
        }
    }

    public static IPropertyDefinition<OrientedStack, Orientation> OrientationPropertyDefinition { get; }

    public QuickBinding<Orientation> OrientationBinding
    {
        set
        {
            value.Bind(OrientationProperty);
        }
    }

    public double Spacing
    {
        get
        {
            return SpacingProperty.CurrentValue;
        }
        set
        {
            if (!EqualityComparer<double>.Default.Equals(SpacingProperty.CurrentValue, value))
            {
                SpacingProperty.CurrentValue = value;
            }
        }
    }

    public static IPropertyDefinition<OrientedStack, double> SpacingPropertyDefinition { get; }

    public QuickBinding<double> SpacingBinding
    {
        set
        {
            value.Bind(SpacingProperty);
        }
    }

    static OrientedStack()
    {
        LengthTypeProperty = new AttachedProperty<DependencyObject, GridUnitType>(GridUnitType.Auto);
        LengthValueProperty = new AttachedProperty<DependencyObject, double>(0.0);
        LengthProperty = new AttachedProperty<DependencyObject, GridLength>(default(GridLength));
        VisibilityTrackingProperty = new AttachedProperty<DependencyObject, bool>(defaultValue: false);
        VisibilityPropertyDefinition = UIElement.VisibilityProperty.AsPropertyDefinition<UIElement, Visibility>();
        OrientationPropertyDefinition = new PropertyDefinition<OrientedStack, Orientation>((OrientedStack x) => x.OrientationProperty);
        SpacingPropertyDefinition = new PropertyDefinition<OrientedStack, double>((OrientedStack x) => x.SpacingProperty);
        LengthTypeProperty.ValueChanged += OnLengthTypeChanged;
        LengthValueProperty.ValueChanged += OnLengthValueChanged;
        LengthProperty.ValueChanged += OnLengthChanged;
    }

    public OrientedStack()
    {
        OrientationProperty.ValueChanged += OnOrientationChanged;
    }

    public OrientedStack(Orientation orientation = Orientation.Vertical, double spacing = 0.0)
        : this()
    {
        Orientation = orientation;
        Spacing = spacing;
    }

    private static void OnLengthTypeChanged(DependencyObject obj, GridUnitType oldValue, GridUnitType newValue)
    {
        GridLength gridLength = new GridLength(LengthProperty.GetValue(obj).Value, newValue);
        if (LengthProperty.GetValue(obj) != gridLength)
        {
            LengthProperty.SetValue(obj, gridLength);
        }
    }

    private static void OnLengthValueChanged(DependencyObject obj, double oldValue, double newValue)
    {
        GridLength gridLength = new GridLength(newValue, LengthProperty.GetValue(obj).GridUnitType);
        if (LengthProperty.GetValue(obj) != gridLength)
        {
            LengthProperty.SetValue(obj, gridLength);
        }
    }

    private static void OnLengthChanged(DependencyObject obj, GridLength oldValue, GridLength newValue)
    {
        if (LengthValueProperty.GetValue(obj) != newValue.Value)
        {
            LengthValueProperty.SetValue(obj, newValue.Value);
        }

        if (LengthTypeProperty.GetValue(obj) != newValue.GridUnitType)
        {
            LengthTypeProperty.SetValue(obj, newValue.GridUnitType);
        }

        (VisualTreeHelper.GetParent(obj) as OrientedStack)?.InvalidateMeasure();
        (VisualTreeHelper.GetParent(obj) as OrientedStack)?.InvalidateArrange();
    }

    private void OnOrientationChanged(Orientation oldValue, Orientation newValue)
    {
        InvalidateArrange();
        InvalidateMeasure();
    }

    private void OnChildVisibilityChanged(Visibility _, Visibility _1)
    {
        InvalidateArrange();
        InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (base.Tag is string text && text == "Debug")
        {
            Debugger.Break();
        }

        Orientation orientation = Orientation;
        (double, double) tuple = SizeToOF(availableSize);
        (double, double) of = tuple;
        double num = 0.0;
        int count = base.Children.Count;
        List<(double, UIElement)> list = new List<(double, UIElement)>(count);
        List<UIElement> list2 = new List<UIElement>(count);
        List<(double, UIElement)> list3 = new List<(double, UIElement)>(count);
        List<(double, UIElement)> list4 = new List<(double, UIElement)>(count);
        double num2 = 0.0;
        double num3 = 0.0;
        double num4 = 0.0;
        int num5 = 0;
        foreach (UIElement child in base.Children)
        {
            if (VisibilityTrackingProperty.GetValue(child) && !CachedChildren.Remove(child))
            {
                IProperty<Visibility> property = VisibilityPropertyDefinition.GetProperty(child);
                CachedChildrenVisibility[child] = property;
                property.ValueChanged += OnChildVisibilityChanged;
            }

            if (child.Visibility == Visibility.Visible)
            {
                num5++;
                GridLength value = LengthProperty.GetValue(child);
                if (value.IsAuto)
                {
                    list2.Add(child);
                }
                else if (value.IsStar)
                {
                    num3 += value.Value;
                    list3.Add((value.Value, child));
                }
                else
                {
                    num2 += value.Value;
                    list4.Add((value.Value, child));
                }
            }
        }

        foreach (UIElement cachedChild in CachedChildren)
        {
            if (CachedChildrenVisibility.Remove(cachedChild, out IProperty<Visibility> value2))
            {
                value2.ValueChanged -= OnChildVisibilityChanged;
            }

            if (value2 is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        CachedChildren.Clear();
        if (num5 > 1)
        {
            num2 += (double)(num5 - 1) * Spacing;
        }

        foreach (var (item, uIElement) in list)
        {
            uIElement.Measure(OFToSize((Along: item, Opposite: tuple.Item2)));
            double item2 = SizeToOF(uIElement.DesiredSize).Opposite;
            if (item2 > num4)
            {
                num4 = item2;
            }
        }

        of.Item1 -= num2;
        num += num2;
        of.Item1 = Math.Max(of.Item1, 0.0);
        foreach (UIElement item3 in list2)
        {
            item3.Measure(OFToSize(of));
            var (num6, num7) = SizeToOF(item3.DesiredSize);
            if (num7 > num4)
            {
                num4 = num7;
            }

            num += num6;
            of.Item1 -= num6;
            of.Item1 = Math.Max(of.Item1, 0.0);
        }


        foreach (var (pixel, child) in list4)
        {
            child.Measure(OFToSize((pixel, of.Item2)));
        }
        
        double num8 = 0.0;
        foreach (var (num9, uIElement2) in list3)
        {
            uIElement2.Measure(OFToSize(of));
            var (num10, num11) = SizeToOF(uIElement2.DesiredSize);
            if (num11 > num4)
            {
                num4 = num11;
            }

            double num12 = num10 / num9;
            if (num12 > num8)
            {
                num8 = num12;
            }
        }

        double num13 = num8 * num3;
        of.Item1 -= num13;
        num += num13;
        of.Item1 = Math.Max(of.Item1, 0.0);
        Size result = OFToSize((Along: num, Opposite: Math.Min(num4, tuple.Item2)));
        if (base.Tag is string text2 && text2 == "Debug")
        {
            Debugger.Break();
        }

        return result;
        Size OFToSize((double Along, double Opposite) tuple6)
        {
            return (orientation == Orientation.Horizontal) ? new Size(tuple6.Along, tuple6.Opposite) : new Size(tuple6.Opposite, tuple6.Along);
        }

        (double Along, double Opposite) SizeToOF(Size size)
        {
            return (orientation == Orientation.Horizontal) ? (Along: size.Width, Opposite: size.Height) : (Along: size.Height, Opposite: size.Width);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (base.Tag is string text && text == "Debug")
        {
            Debugger.Break();
        }

        Orientation orientation = Orientation;
        (double, double) tuple = SizeToOF(finalSize);
        (double, double) tuple2 = tuple;
        double num = 0.0;
        double num2 = 0.0;
        int num3 = 0;
        foreach (UIElement child in base.Children)
        {
            if (child.Visibility == Visibility.Visible)
            {
                num3++;
                GridLength value = LengthProperty.GetValue(child);
                if (value.IsAuto)
                {
                    num += SizeToOF(child.DesiredSize).Along;
                }
                else if (value.IsStar)
                {
                    num2 += value.Value;
                }
                else
                {
                    num += value.Value;
                }
            }
        }

        if (num3 > 1)
        {
            num += (double)(num3 - 1) * Spacing;
        }

        tuple2.Item1 -= num;
        tuple2.Item1 = Math.Max(tuple2.Item1, 0.0);
        double num4 = 0.0;
        if (num2 == 0.0)
        {
            num2 = 1.0;
        }

        double num5 = tuple2.Item1 / num2;
        foreach (UIElement child2 in base.Children)
        {
            if (child2.Visibility == Visibility.Visible)
            {
                GridLength value2 = LengthProperty.GetValue(child2);
                if (value2.IsAuto)
                {
                    (double, double) tuple3 = SizeToOF(child2.DesiredSize);
                    child2.Arrange(new Rect(OFToPoint((Along: num4, Opposite: 0.0)), OFToSize((Along: tuple3.Item1, Opposite: tuple2.Item2))));
                    num4 += tuple3.Item1 + Spacing;
                    tuple2.Item1 -= tuple3.Item1 + Spacing;
                }
                else if (value2.IsStar)
                {
                    double num6 = num5 * value2.Value;
                    child2.Arrange(new Rect(OFToPoint((Along: num4, Opposite: 0.0)), OFToSize((Along: num6, Opposite: tuple2.Item2))));
                    num4 += num6 + Spacing;
                    tuple2.Item1 -= num6 + Spacing;
                }
                else
                {
                    child2.Arrange(new Rect(OFToPoint((Along: num4, Opposite: 0.0)), OFToSize((Along: value2.Value, Opposite: tuple2.Item2))));
                    num4 += value2.Value + Spacing;
                    tuple2.Item1 -= value2.Value + Spacing;
                }
            }
        }

        tuple2.Item1 = Math.Max(tuple2.Item1, 0.0);
        if (base.Tag is string text2 && text2 == "Debug")
        {
            Debugger.Break();
        }

        return OFToSize((Along: tuple.Item1 - tuple2.Item1, Opposite: Math.Min(tuple2.Item2, tuple.Item2)));
        Point OFToPoint((double Along, double Opposite) of)
        {
            return (orientation == Orientation.Horizontal) ? new Point(of.Along, of.Opposite) : new Point(of.Opposite, of.Along);
        }

        Size OFToSize((double Along, double Opposite) of)
        {
            return (orientation == Orientation.Horizontal) ? new Size(of.Along, of.Opposite) : new Size(of.Opposite, of.Along);
        }

        (double Along, double Opposite) SizeToOF(Size size)
        {
            return (orientation == Orientation.Horizontal) ? (Along: size.Width, Opposite: size.Height) : (Along: size.Height, Opposite: size.Width);
        }
    }
}
