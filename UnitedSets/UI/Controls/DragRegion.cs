using Get.Data.XACL;
using Windows.Graphics;
namespace UnitedSets.UI.Controls;

[AttachedProperty(typeof(bool), "Clickable", typeof(FrameworkElement), GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(NonClientRegionKind), "ClientKind", typeof(FrameworkElement), DefaultValueExpression = "(global::Microsoft.UI.Input.NonClientRegionKind)10", GenerateLocalOnPropertyChangedMethod = true)]
public partial class UnitedSetsDragRegion : Grid
{
    public UnitedSetsDragRegion()
    {
        SizeChanged += DragRegion_SizeChanged;
        Loaded += DragRegion_Loaded;
        Unloaded += DragRegion_Unloaded;
    }

    private void XamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        UpdateRegion();
    }
    readonly static Dictionary<object, UnitedSetsDragRegion> mapping = [];
    static partial void OnClickableChanged(FrameworkElement obj, bool oldValue, bool newValue)
    {
        if (newValue)
        {
            obj.SizeChanged -= Obj_SizeChanged;
            obj.Loaded -= Obj_Loaded;
            obj.Unloaded -= Obj_Unloaded;
            obj.SizeChanged += Obj_SizeChanged;
            obj.Loaded += Obj_Loaded;
            obj.Unloaded += Obj_Unloaded;
            Update(obj);
        }
        else if (oldValue)
        {
            obj.SizeChanged -= Obj_SizeChanged;
            obj.Loaded -= Obj_Loaded;
            obj.Unloaded -= Obj_Unloaded;
            Update(obj);
            mapping.Remove(obj);
        }
    }

    private static void Obj_Unloaded(object sender, RoutedEventArgs e) => Update(sender);

    private static void Obj_Loaded(object sender, RoutedEventArgs e) => Update(sender);
    static void Obj_SizeChanged(object sender, SizeChangedEventArgs e) => Update(sender);
    static void Update(object sender)
    {
        var dragRegion = (sender as DependencyObject)?.FindAscendant<UnitedSetsDragRegion>();
        if (dragRegion is null)
        {
            if (!mapping.TryGetValue(sender, out dragRegion))
                return;
        }
        else
            mapping[sender] = dragRegion;
        dragRegion?.UpdateRegion();
    }

    private void DragRegion_Unloaded(object sender, RoutedEventArgs e)
    {
        XamlRoot.Changed -= XamlRoot_Changed;
        try
        {
            current?.ClearAllRegionRects();
        }
        catch
        {
            current = null;
        }
    }

    private void DragRegion_Loaded(object sender, RoutedEventArgs e)
    {
        XamlRoot.Changed -= XamlRoot_Changed;
        XamlRoot.Changed += XamlRoot_Changed;
        current?.ClearAllRegionRects();
        current = InputNonClientPointerSource.GetForWindowId(XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    private void DragRegion_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateRegion();
    }

    InputNonClientPointerSource? current;
    public void UpdateRegion()
    {
        try
        {
            current?.SetRegionRects(NonClientRegionKind.Caption,
                [new(0, 0, (int)ActualWidth, (int)ActualHeight)]
            );
            List<RectInt32> passthrough = [];
            List<(RectInt32, NonClientRegionKind)> custom = [];
            GetClickableRectangles(this, this, passthrough, custom);
            current?.SetRegionRects(NonClientRegionKind.Passthrough, passthrough.ToArray());
            foreach (var (rect, kind) in custom)
                current?.SetRegionRects(kind, new RectInt32[] { rect });
        }
        catch
        {
            current = null;
        }
    }
    static void GetClickableRectangles(UIElement element, UIElement relativeTo, List<RectInt32> passthrough, List<(RectInt32, NonClientRegionKind)> custom)
    {
        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is FrameworkElement fe)
            {
                if (GetClickable(fe))
                    goto clickthrough;
                var clientKind = GetClientKind(fe);
                if (clientKind is not (NonClientRegionKind)10)
                {
                    custom.Add((GetRect(fe, relativeTo), clientKind));
                    continue;
                }
            }
            if (child switch
            {
                Panel panel => panel.Background == null,
                Control control => control.Background == null,
                Border border => border.Background == null,
                ContentPresenter contentPresenter => contentPresenter.Background == null,
                TextBlock or Image => true,
                _ => false
            })
                goto clickthrough;
            else
                goto clickable;
clickthrough:
            GetClickableRectangles((UIElement)child, relativeTo, passthrough, custom);
            continue;
clickable:
            passthrough.Add(GetRect((UIElement)child, relativeTo));
        }
    }
    static RectInt32 GetRect(UIElement element, UIElement relativeTo)
    {
        var pt = element.TransformToVisual(relativeTo).TransformPoint(default);
        return new() { X = (int)pt.X, Y = (int)pt.Y, Width = (int)element.ActualSize.X, Height = (int)element.ActualSize.Y };
    }
}
