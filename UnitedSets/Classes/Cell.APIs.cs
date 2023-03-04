using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Linq;
using System;
using Window = WinWrapper.Window;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;
namespace UnitedSets.Classes;
partial class Cell
{
    public void RegisterWindow(Window Window)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();
        CurrentCell = new(MainWindow, Window);
    }

    public void SplitHorizontally(int Amount)
    {
        if (!Empty) throw new InvalidOperationException();
        Orientation = Orientation.Vertical;
        SubCells = CraeteNCells(Amount);
    }
    
    public void SplitVertically(int Amount)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();
        Orientation = Orientation.Horizontal;
        SubCells = CraeteNCells(Amount);
    }
    
    Cell[] CraeteNCells(int Amount)
    {
        return (from _ in 0..Amount select new Cell(MainWindow, null, null, default)).ToArray();
    }

    public Cell DeepClone(MainWindow NewWindow)
    {
        Cell[]? newSubCells =
            SubCells is null ? null :
            (from x in SubCells select x.DeepClone(NewWindow)).ToArray();
        HwndHost? hwndHost =
            CurrentCell is null ? null
            : new HwndHost(NewWindow, CurrentCell.HostedWindow);
        Cell cell = new(NewWindow, hwndHost, newSubCells, Orientation);
        return cell;
    }
}