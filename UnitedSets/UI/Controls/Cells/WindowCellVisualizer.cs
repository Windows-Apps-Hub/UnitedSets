using UnitedSets.Cells;
using WindowHoster;

namespace UnitedSets.UI.Controls.Cells;

[QuickMarkup("""
    double CellMargin;
    <root
        Margin=`new(CellMargin)`
        AssociatedWindow = `cell.Window`
    />
    """)]
public partial class WindowCellVisualizer : WindowHost
{
    readonly WindowCell cell;
    public WindowCellVisualizer(WindowCell cell)
    {
        this.cell = cell;
        Init();
    }
}
