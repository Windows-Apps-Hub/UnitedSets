using System.ComponentModel;
using Microsoft.UI.Xaml;
using System.Linq;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Window = WinWrapper.Window;
using EasyCSharp;
using UnitedSets.Windows;
namespace UnitedSets.Classes;
partial class Cell
{
    [Event(typeof(DragEventHandler), Name = "DragOverEv")]
    public void OnDragOver(DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty || !e.DataView.Properties.ContainsKey(MainWindow.UnitedSetsTabWindowDragProperty)) return;
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    [Event(typeof(DragEventHandler), Name = "DropEv")]
    public void OnItemDrop(DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty || !e.DataView.Properties.TryGetValue(MainWindow.UnitedSetsTabWindowDragProperty, out var _a) || _a is long hwnd == false)
			return;
		ValidDrop?.Invoke(this, new ValidItemDropArgs(hwnd));

    }
	public class ValidItemDropArgs : EventArgs {
		public ValidItemDropArgs(long HwndId) => this.HwndId = HwndId;
		public long HwndId;
	}
	public static event EventHandler<ValidItemDropArgs>? ValidDrop;

	[Event(typeof(RoutedEventHandler), Name = "AddCellAddCountClickEv")]
    public void AddCellAddCount()
    {
        CellAddCount += 1;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCount)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCountAsString)));
    }

    [Event(typeof(RoutedEventHandler), Name = "SubtractCellAddCountClickEv")]
    public void SubtractCellAddCount()
    {
        if (CellAddCount <= 2) return;
        CellAddCount -= 1;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCount)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCountAsString)));
    }

    [Event(typeof(RoutedEventHandler), Name = "SplitVerticallyClickEv", Visibility = GeneratorVisibility.Public)]
    void SplitVerticallyCellAddCount()
    {
        SplitVertically(CellAddCount);
    }

    [Event(typeof(RoutedEventHandler), Name = "SplitHorizontallyClickEv", Visibility = GeneratorVisibility.Public)]
    void SplitHorizontallyCellAddCount()
    {
        SplitHorizontally(CellAddCount);
    }
}
