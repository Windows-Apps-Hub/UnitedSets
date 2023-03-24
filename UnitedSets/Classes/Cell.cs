using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Linq;
using System;
using Window = WinWrapper.Window;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.UI.AppWindows;
namespace UnitedSets.Classes;
public partial class Cell : INotifyPropertyChanged
{
    public Cell(OurHwndHost? CurrentCell, Cell[]? SubCells, Orientation Orientation)
    {
        _CurrentCell = CurrentCell;
        _SubCells = SubCells;
        _Orientation = Orientation;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}