using Microsoft.UI.Xaml.Controls;
namespace UnitedSets.Classes.Tabs;

public partial class CellTab : TabBase {

	public CellTab(bool IsTabSwitcherVisibile = DefaultIsSwitcherVisible)
        : this(
              new(null, null, Orientation.Horizontal),
              IsTabSwitcherVisibile
        )
    {
    }
    
    protected CellTab(Cell Cell, bool IsTabSwitcherVisibile = DefaultIsSwitcherVisible) : base(IsTabSwitcherVisibile)
    {
        _MainCell = Cell;
    }
}
