using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace UnitedSets.Classes;

partial class Cell
{
    public bool ContainsWindow => CurrentCell is not null;
    public bool ContainsSubCells => SubCells is not null;
    public bool IsEmpty => !(ContainsWindow || ContainsSubCells);

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