using Microsoft.UI.Xaml.Controls;
namespace UnitedSets.Classes;
public partial class Cell
{
    public Cell(OurHwndHost? CurrentCell, Cell[]? SubCells, Orientation Orientation)
    {
        _CurrentCell = CurrentCell;
        _SubCells = SubCells;
        _Orientation = Orientation;
    }
}