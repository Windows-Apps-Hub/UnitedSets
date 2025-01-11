using System.Collections.Generic;
using System.Linq;
using Get.Data.Collections;
using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
using Get.Data.Properties;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Tabs;
using static Get.Data.Properties.AutoTyper;
namespace UnitedSets.Cells;
[AutoProperty]
public partial class ContainerCell : Cell
{
    public CellTab? ParentCellTab { get; set; }
    public ContainerCell(ContainerCell? Parent, Orientation Orientation) : base(Parent)
    {
        this.Orientation = Orientation;
    }
    public IUpdateCollection<Cell> SubCells { get; } = new UpdateCollection<Cell>();
    public IProperty<Orientation> OrientationProperty { get; } = Auto<Orientation>(default);

    public (Cell?, double renamining) GetChildFromPosition(double normalizedPosition)
    {
        if (SubCells is null) return (null, 0);
        var RSes = SubCells.AsEnumerable().Select(x => (x, x.RelativeSize)).ToArray();
        var RStotal = RSes.Sum(x => x.RelativeSize);
        var posInRSScale = normalizedPosition * RStotal;
        foreach (var (cell, rs) in RSes)
        {
            if (posInRSScale < rs) return (cell, posInRSScale / RStotal);
            posInRSScale -= rs;
        }
        return (null, 0);
    }

    public (double In1, double In2) TranslatePositionFromChild((double In1, double In2) a, Cell childCell)
    {
        if (SubCells is null) return a;
        var RSes = SubCells.AsEnumerable().Select(x => (x, x.RelativeSize)).ToArray();
        var RStotal = RSes.Sum(x => x.RelativeSize);
        var front = 0d;
        foreach (var (cell, rs) in RSes)
        {
            if (cell == childCell) return (front / RStotal + a.In1, front / RStotal + a.In2);
            front += rs;
        }
        return a;
    }
    public IEnumerable<Cell> AllSubCells
    {
        get
        {
            yield return this;
            if (SubCells is null) yield break;
            foreach (var cell in SubCells.AsEnumerable())
                if (cell is ContainerCell cc)
                    foreach (var cellsubcell in cc.AllSubCells)
                        yield return cellsubcell;
                else yield return cell;
        }
    }
}
