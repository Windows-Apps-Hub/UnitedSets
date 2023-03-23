using Microsoft.UI.Xaml.Controls;
using UnitedSets.Windows;
using EasyCSharp;
namespace UnitedSets.Classes.Tabs;

public partial class CellTab : TabBase, IHwndHostParent {

	TabBase IHwndHostParent.Tab => this;
    
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
