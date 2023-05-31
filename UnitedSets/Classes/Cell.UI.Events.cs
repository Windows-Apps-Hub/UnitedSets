using System.ComponentModel;
using Microsoft.UI.Xaml;
using System.Linq;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Window = WinWrapper.Window;
using EasyCSharp;
using UnitedSets.UI.AppWindows;
using CommunityToolkit.Mvvm.Input;

namespace UnitedSets.Classes;
partial class Cell
{
    [RelayCommand]
    public partial void AddCellAddCount();
    [RelayCommand]
    public partial void SubtractCellAddCount();
    [Event(typeof(RoutedEventHandler), Name = "SplitVerticallyClickEv", Visibility = GeneratorVisibility.Public)]
    private partial void SplitVerticallyCellAddCount();
    [Event(typeof(RoutedEventHandler), Name = "SplitHorizontallyClickEv", Visibility = GeneratorVisibility.Public)]
    private partial void SplitHorizontallyCellAddCount();


    [Event(typeof(DragEventHandler), Name = "DragOverEv")]
    public void OnDragOver(DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty || !e.DataView.Properties.ContainsKey(Constants.UnitedSetsTabWindowDragProperty)) return;
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    [Event(typeof(DragEventHandler), Name = "DropEv")]
    public void OnItemDrop(DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty || !e.DataView.Properties.TryGetValue(Constants.UnitedSetsTabWindowDragProperty, out var _a) || _a is long hwnd == false)
			return;
		ValidDrop?.Invoke(this, new ValidItemDropArgs(hwnd));

    }
	public class ValidItemDropArgs : EventArgs {
		public ValidItemDropArgs(long HwndId) => this.HwndId = HwndId;
		public long HwndId;
	}
	public static event EventHandler<ValidItemDropArgs>? ValidDrop;

	public partial void AddCellAddCount()
    {
        CellAddCount += 1;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCount)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCountAsString)));
    }

    public partial void SubtractCellAddCount()
    {
        if (CellAddCount <= 2) return;
        CellAddCount -= 1;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCount)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCountAsString)));
    }

    private partial void SplitVerticallyCellAddCount()
        => SplitVertically(CellAddCount);

    private partial void SplitHorizontallyCellAddCount()
        => SplitHorizontally(CellAddCount);
}
