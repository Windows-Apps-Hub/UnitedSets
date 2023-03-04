using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace UnitedSets.Classes;

partial class Cell
{
    public bool HasWindow => CurrentCell is not null;
    public bool HasSubCells => SubCells is not null;
    public bool HasVerticalSubCells => HasSubCells && Orientation == Orientation.Vertical;
    public bool HasHorizontalSubCells => HasSubCells && Orientation == Orientation.Horizontal;

    public bool Empty => !(HasWindow || HasSubCells);

    public IEnumerable<Cell> AllSubCells
    {
        get
        {
            yield return this;
            if (SubCells is null) yield break;
            foreach (var cell in SubCells)
                foreach (var cellsubcell in cell.AllSubCells)
                    yield return cellsubcell;
        }
    }
}