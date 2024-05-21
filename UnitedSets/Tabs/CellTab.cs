using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
namespace UnitedSets.Tabs;

public partial class CellTab : TabBase {

	public CellTab(bool IsTabSwitcherVisibile = DefaultIsSwitcherVisible)
        : this(
              new(null, null, Orientation.Horizontal),
              IsTabSwitcherVisibile
        )
    {
    }
    
    public CellTab(Cell Cell, bool IsTabSwitcherVisibile = DefaultIsSwitcherVisible) : base(IsTabSwitcherVisibile)
    {
        _MainCell = Cell;
    }
}
