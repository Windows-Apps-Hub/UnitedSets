using Get.Data.Collections;
using Get.Data.Properties;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
namespace UnitedSets.Tabs;
[AutoProperty]
public partial class CellTab(ContainerCell Cell, bool IsTabSwitcherVisibile = TabBase.DefaultIsSwitcherVisible) : TabBase(IsTabSwitcherVisibile) {

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
