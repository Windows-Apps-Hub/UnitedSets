using System;
using Get.Data.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Apps;
using WindowHoster;

namespace UnitedSets.Cells;
public partial class EmptyCell(ContainerCell Parent) : Cell(Parent)
{

    /// <summary>
    /// Removes this cell and replace with a ContainerCell with <paramref name="amount"/> EmptyCell and given <paramref name="orientation"/>
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="orientation"></param>
    /// <exception cref="InvalidOperationException">
    /// Throws if this cell is in an invalid state.
    /// </exception>
    public void Split(int amount, Orientation orientation)
    {
        if (Parent is null) throw new InvalidOperationException();
        int idx = Parent.SubCells.IndexOf(this);
        if (idx < 0) throw new InvalidOperationException();
        var newCell = new ContainerCell(Parent, orientation);
        for (int i = 0; i < amount; i++)
            newCell.SubCells.Add(new EmptyCell(Parent: newCell));
        Parent.SubCells[idx] = newCell;
    }

    /// <summary>
    /// Removes this cell and replace with a WindowCell with given <paramref name="window"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Throws if this cell is in an invalid state.
    /// </exception>
    public void RegisterWindow(RegisteredWindow window)
    {
        if (Parent is null) throw new InvalidOperationException();
        int idx = Parent.SubCells.IndexOf(this);
        if (idx < 0) throw new InvalidOperationException();
        var newCell = new WindowCell(Parent, window);
        Parent.SubCells[idx] = newCell;
    }

    public void OnItemDrop(object? _, DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!e.DataView.Properties.TryGetValue(Constants.UnitedSetsTabWindowDragProperty, out var _a) || _a is long hwnd == false)
            return;
        EmptyCell.ValidDrop?.Invoke(this, (nint)hwnd);
    }

    public static event ValidItemDropEventHandler? ValidDrop;
    public delegate void ValidItemDropEventHandler(EmptyCell sender, nint HwndId);
}
