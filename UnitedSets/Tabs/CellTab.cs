using Get.Data.Collections;
using Get.Data.Properties;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Cells;
namespace UnitedSets.Tabs;
[AutoProperty]
public partial class CellTab : TabBase
{
    public CellTab(ContainerCell Cell, bool IsTabSwitcherVisibile = TabBase.DefaultIsSwitcherVisible) : base(IsTabSwitcherVisibile)
    {
        Cell.ParentCellTab = this;
        MainCellProperty = Auto(Cell);
    }
    public CellTab(bool IsTabSwitcherVisibile = DefaultIsSwitcherVisible)
    : this(
          CreateEmpty(),
          IsTabSwitcherVisibile
    )
    {
    }
    static ContainerCell CreateEmpty()
    {
        ContainerCell cell = new(null, Orientation.Horizontal);
        cell.SubCells.Add(new EmptyCell(Parent: cell));
        return cell;
    }
}
