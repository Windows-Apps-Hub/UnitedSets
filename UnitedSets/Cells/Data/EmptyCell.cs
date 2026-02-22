using UnitedSets.Apps;
using WindowHoster;

namespace UnitedSets.Cells.Data;
public partial class EmptyCell(ContainerCell Parent) : Cell(Parent)
{
    public partial void Split(int amount, Orientation orientation)
    {
        if (Parent is null) throw new InvalidOperationException();
        int idx = Parent.SubCells.IndexOf(this);
        if (idx < 0) throw new InvalidOperationException();
        var newCell = new ContainerCell(Parent, orientation);
        for (int i = 0; i < amount; i++)
            newCell.SubCells.Add(new EmptyCell(Parent: newCell));
        Parent.SubCells[idx] = newCell;
    }

    public partial void RegisterWindow(RegisteredWindow window)
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
