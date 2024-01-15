using System.ComponentModel;
using Microsoft.UI.Xaml;
using System.Linq;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Window = WinWrapper.Windowing.Window;
using Get.EasyCSharp;
using UnitedSets.UI.AppWindows;
using CommunityToolkit.Mvvm.Input;

namespace UnitedSets.Classes;
partial class Cell
{
    [RelayCommand]
    public partial void AddCellAddCount();
    [RelayCommand]
    public partial void SubtractCellAddCount();


    [Event(typeof(DragEventHandler), Name = "DragOverEv")]
    public void OnDragOver(DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!IsEmpty || !e.DataView.Properties.ContainsKey(Constants.UnitedSetsTabWindowDragProperty)) return;
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    [Event(typeof(DragEventHandler), Name = "DropEv")]
    public void OnItemDrop(DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!IsEmpty || !e.DataView.Properties.TryGetValue(Constants.UnitedSetsTabWindowDragProperty, out var _a) || _a is long hwnd == false)
			return;
		ValidDrop?.Invoke(this, (nint)hwnd);

    }
	public static event ValidItemDropEventHandler? ValidDrop;
    public delegate void ValidItemDropEventHandler(Cell sender, nint HwndId);

	public partial void AddCellAddCount()
    {
        CellAddCount += 1;
        NotifyPropertyChanged(nameof(CellAddCount));
    }

    public partial void SubtractCellAddCount()
    {
        if (CellAddCount <= 2) return;
        CellAddCount -= 1;
        NotifyPropertyChanged(nameof(CellAddCount));
    }
}
