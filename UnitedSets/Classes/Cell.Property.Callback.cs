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
                CurrentCell = null;
            };
            _CurrentCell.IsWindowVisible = IsVisible;
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
        if (CurrentCell is HwndHost hwndHost) hwndHost.IsWindowVisible = IsVisible;
        else PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
    }
}