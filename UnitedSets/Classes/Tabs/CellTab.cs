using Microsoft.UI.Xaml.Controls;
using UnitedSets.Windows;

namespace UnitedSets.Classes.Tabs;

public partial class CellTab : TabBase
{
    readonly MainWindow MainWindow;
    
    public CellTab(MainWindow MainWindow, bool IsTabSwitcherVisibile)
        : this(
              MainWindow,
              new(MainWindow, null, null, Orientation.Horizontal),
              IsTabSwitcherVisibile
        )
    {
    }
    
    protected CellTab(MainWindow MainWindow, Cell Cell, bool IsTabSwitcherVisibile)
        : base(MainWindow.TabView, IsTabSwitcherVisibile)
    {
        this.MainWindow = MainWindow;
        _MainCell = Cell;
    }
}
