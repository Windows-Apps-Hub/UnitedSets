using UnitedSets.Cells;
namespace UnitedSets.UI.Controls.Cells;
[QuickMarkup("""
    using UnitedSets.Cells;
    using UnitedSets.Controls;
    Cell Cell;
    double CellMargin;
    private ContainerCell? CellParent => `Cell?.Parent`;
    private bool ShouldShowResizer => `() => {
        if (Cell?.Parent is not { } parent) return false;
        var idx = parent.SubCells.IndexOf(Cell);
        // don't show the resizer if it's the last element
        // (or if somehow idx is invalid)
        return idx >= 0 && idx < parent.SubCells.Count - 1;
    }`;
    <root
        StretchH StretchV
        LengthValue=`Cell.RelativeSize`
        Orientation=`CellParent?.Orientation ?? default(Orientation)`
    >
        visContainer = <Border Child=`CreateVisualizer(Cell)` />
        // resizer not working due to https://github.com/CommunityToolkit/Windows/issues/273
        resizer = <OrientedStackResizer
            Canvas_ZIndex=100
            Background=`Solid(Colors.Red)`
            MinWidth=5 MinHeight = 5
            IsEnabled
            Orientation=`CellParent?.Orientation.Flip() ?? default(Orientation)`
            IsVisible=`ShouldShowResizer`
        />
    </root>
    """)]
public partial class GenericCellVisualizer : OrientedStack
{
    private double LengthValue
    {
        set => OrientedStack.LengthProperty.SetValue(this, new(value, GridUnitType.Star));
    }
    public GenericCellVisualizer(Cell cell)
    {
        Cell = cell;
        Init();
        OrientedStack.LengthProperty.SetValue(visContainer, Star(1));
    }

    UIElement CreateVisualizer(Cell x) => ReferenceTracker.NoCapture<UIElement>(() =>
    {
        if (x is EmptyCell ec)
            return new EmptyCellVisualizer(ec);
        else if (x is ContainerCell cc)
        {
            var a = new CellContainerVisualizer(cc);
            CellMarginProp.Watch(x => a.CellMargin = x, immediete: true);
            return a;
        }
        else if (x is WindowCell wc)
        {
            var a = new WindowCellVisualizer(wc);
            CellMarginProp.Watch(x => a.CellMargin = x, immediete: true);
            return a;
        }
        throw new System.InvalidCastException("Cannot infer type");
    });
}
