using Microsoft.UI.Xaml.Controls;
using UnitedSets.Windows;

namespace UnitedSets.Classes.Tabs;

public partial class CellTab : TabBase, IHwndHostParent {

	TabBase IHwndHostParent.Tab => this;
	public CellTab(bool IsTabSwitcherVisibile)
        : this(
              new(null, null, Orientation.Horizontal),
              IsTabSwitcherVisibile
        )
    {
    }
    
    public CellTab(Cell Cell, bool IsTabSwitcherVisibile)
        : base(IsTabSwitcherVisibile)
    {
        _MainCell = Cell;
    }
}
