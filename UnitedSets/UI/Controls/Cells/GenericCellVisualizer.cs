using Get.Data.Collections;
using Get.Data.Properties;
using Get.UI.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Cells;
namespace UnitedSets.UI.Controls.Cells;
[AutoProperty]
public partial class GenericCellVisualizer(Cell cell) : TemplateControl<OrientedStack>
{
    public IProperty<Cell> CellProperty { get; } = Auto(cell);
    protected override void Initialize(OrientedStack rootElement)
    {
        rootElement.HorizontalAlignment = HorizontalAlignment.Stretch;
        rootElement.VerticalAlignment = VerticalAlignment.Stretch;
        OrientedStack.LengthTypeProperty.SetValue(this, GridUnitType.Star);
        OrientedStack.LengthValueProperty
            .GetProperty(this)
            .BindOneWay(cell.RelativeSizeProperty);
        var visContainer = new Border();
        OrientedStack.LengthProperty.SetValue(visContainer, Star(1));
        // resizer not working due to https://github.com/CommunityToolkit/Windows/issues/273
        //var resizer = new OrientedStackResizer
        //{
        //    Background = Solid(Colors.Red),
        //    MinWidth = 5,
        //    MinHeight = 5,
        //    IsEnabled = true
        //};
        //Canvas.SetZIndex(resizer, 100);
        rootElement.Children.Add(visContainer);
        //rootElement.Children.Add(resizer);
        CellProperty.ApplyAndRegisterForNewValue((_, x) =>
        {
            if (cell.Parent is { } parent)
            {
                rootElement.Orientation = parent.Orientation;
                //resizer.Orientation = rootElement.Orientation is Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
                var idx = parent.SubCells.IndexOf(x);
                // don't show the resizer if it's the last element
                // (or if somehow idx is invalid)
                //resizer.Visibility = idx >= 0 && idx < parent.SubCells.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                //resizer.Visibility = Visibility.Collapsed;
            }
            visContainer.Child = CreateVisualizer(x);
        });
    }
    UIElement CreateVisualizer(Cell x)
    {
        if (x is EmptyCell ec)
            return new EmptyCellVisualizer(ec);
        else if (x is ContainerCell cc)
            return new CellContainerVisualizer(cc);
        else if (x is WindowCell wc)
            return new WindowCellVisualizer(wc);
        throw new System.InvalidCastException("Cannot infer type");
    }
}
