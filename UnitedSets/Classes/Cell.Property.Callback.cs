using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;

namespace UnitedSets.Classes;
partial class Cell
{
    void OnCurrentCellChanged()
    {
        if (_CurrentCell is not null)
        {
            _CurrentCell.Closed += delegate
            {
                CurrentCell = null!;
            };
			_CurrentCell.SetVisible(IsVisible, false);
			

        }
    }

    void OnSubCellsUpdate()
    {
        if (_SubCells is not null)
            foreach (var cell in _SubCells)
                cell.IsVisible = IsVisible;
    }

    void OnVisibleChanged()
    {
        if (_SubCells is not null)
            foreach (var cell in _SubCells)
                cell.IsParentVisible = IsVisible;
		if (CurrentCell != null)
			CurrentCell.SetVisible(IsVisible); 
		else
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));

	}
}
