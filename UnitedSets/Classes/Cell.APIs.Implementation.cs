using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System;
using EasyCSharp;
using UnitedSets.UI.AppWindows;
using System.Diagnostics.CodeAnalysis;

namespace UnitedSets.Classes;
partial class Cell
{
    public partial void RegisterHwndHost(OurHwndHost host)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!IsEmpty) throw new InvalidOperationException();
        CurrentCell = host;
    }

    public partial void SplitHorizontally(int Amount)
    {
        if (!IsEmpty) throw new InvalidOperationException();
        Orientation = Orientation.Vertical;
        SubCells = CraeteNCells(Amount);
    }

    public partial void SplitVertically(int Amount)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!IsEmpty) throw new InvalidOperationException();
        Orientation = Orientation.Horizontal;
        SubCells = CraeteNCells(Amount);
    }

    private static partial Cell[] CraeteNCells(int Amount)
        => (from _ in 0..Amount select new Cell(null, null, default)).ToArray();

    public partial (Cell?, double renamining) GetChildFromPosition(double normalizedPosition)
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

    public partial (double In1, double In2) TranslatePositionFromChild((double In1, double In2) a, Cell childCell)
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
    [DoesNotReturn]
    public partial Cell DeepClone(MainWindow NewWindow)
    {
        throw new NotImplementedException();
        //Cell[]? newSubCells =
        //    SubCells is null ? null :
        //    (from x in SubCells select x.DeepClone(NewWindow)).ToArray();
        //HwndHost? hwndHost =
        //    CurrentCell is null ? null
        //    : new HwndHost(NewWindow, CurrentCell.HostedWindow);
        //Cell cell = new(NewWindow, hwndHost, newSubCells, Orientation);
        //return cell;
    }
}