using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Linq;
using System;
using Window = WinWrapper.Window;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;
namespace UnitedSets.Classes;
public partial class Cell : INotifyPropertyChanged
{
    public Cell(MainWindow MainWindow, HwndHost? CurrentCell, Cell[]? SubCells, Orientation Orientation)
    {
        this.MainWindow = MainWindow;
        _CurrentCell = CurrentCell;
        _SubCells = SubCells;
        _Orientation = Orientation;
    }
}