using Microsoft.UI.Xaml.Controls;
using WindowHoster;
namespace UnitedSets.Classes;
public partial class Cell
{
    public Cell(RegisteredWindow? CurrentCell, Cell[]? SubCells, Orientation Orientation)
    {
        _CurrentCell = CurrentCell;
        _SubCells = SubCells;
        _Orientation = Orientation;
    }
}