using System.ComponentModel;
using System.Collections.Generic;

namespace UnitedSets.Classes;
public abstract partial class ICell : INotifyPropertyChanged
{
    public IEnumerable<ICell> AllSubCells
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