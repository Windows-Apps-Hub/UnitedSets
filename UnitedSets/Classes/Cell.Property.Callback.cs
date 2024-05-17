using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using EasyCSharp;
using UnitedSets.UI.AppWindows;

namespace UnitedSets.Classes;
partial class Cell
{
    void OnCurrentCellChanged()
    {
        if (_CurrentCell is not null)
        {
            _CurrentCell.BecomesInvalid += delegate
            {
                CurrentCell = null!;
            };
			//_CurrentCell.SetVisible(IsVisible, false);
        }
    }

    void OnSubCellsUpdate()
    {
        //if (_SubCells is not null)
        //    foreach (var cell in _SubCells)
        //        cell.IsVisible = IsVisible;
    }
}
