using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Linq;
using System;
using Window = WinWrapper.Window;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.UI.AppWindows;
namespace UnitedSets.Classes;
partial class Cell
{
    public void RegisterWindow(OurHwndHost host)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();
        CurrentCell = host;
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
        return (from _ in 0..Amount select new Cell(null, null, default)).ToArray();
    }

    //public Cell DeepClone(MainWindow NewWindow)
    //{
    //    Cell[]? newSubCells =
    //        SubCells is null ? null :
    //        (from x in SubCells select x.DeepClone(NewWindow)).ToArray();
    //    HwndHost? hwndHost =
    //        CurrentCell is null ? null
    //        : new HwndHost(NewWindow, CurrentCell.HostedWindow);
    //    Cell cell = new(NewWindow, hwndHost, newSubCells, Orientation);
    //    return cell;
    //}

    public (Cell?, double renamining) GetChildFromPosition(double normalizedPosition)
    {
        if (SubCells is null) return (null, 0);
        var RSes = SubCells.Select(x => (x, x.RelativeSize)).ToArray();
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
        var RSes = SubCells.Select(x => (x, x.RelativeSize)).ToArray();
        var RStotal = RSes.Sum(x => x.RelativeSize);
        var front = 0d;
        foreach (var (cell, rs) in RSes)
        {
            if (cell == childCell) return (front / RStotal + a.In1, front / RStotal + a.In2);
            front += rs;
        }
        return a;
    }

}