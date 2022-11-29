using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Linq;
using System;
using Window = WinWrapper.Window;
using EasyCSharp;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;
namespace UnitedSets.Classes;
public partial class Cell : ICell, INotifyPropertyChanged
{
    public override MainWindow MainWindow { get; }
    public Cell(MainWindow MainWindow, HwndHost? CurrentCell, Cell[]? SubCells, Orientation Orientation)
    {
        this.MainWindow = MainWindow;
        _CurrentCell = CurrentCell;
        _SubCells = SubCells;
        _Orientation = Orientation;
    }

    [Property(OnChanged = nameof(OnCurrentCellChanged), OverrideKeyword = true)]
    HwndHost? _CurrentCell;
    void OnCurrentCellChanged()
    {
        if (_CurrentCell is not null)
        {
            _CurrentCell.Closed += delegate
            {
                CurrentCell = null;
            };
            _CurrentCell.IsWindowVisible = IsVisible;
        }
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    [Property(OnChanged = nameof(OnSubCellsUpdate), OverrideKeyword = true)]
    Cell[]? _SubCells;
    void OnSubCellsUpdate()
    {
        if (_SubCells is not null)
            foreach (var cell in _SubCells)
                cell.IsVisible = IsVisible;
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    
    [Property(OnChanged = nameof(OnVisibleChanged))]
    bool _IsParentVisible = true;
    
    bool _IsVisible;
    public override bool IsVisible
    {
        get => IsParentVisible && _IsVisible;
        set
        {
            _IsVisible = value;
            OnVisibleChanged();
        }
    }
    void OnVisibleChanged()
    {
        if (_SubCells is not null)
            foreach (var cell in _SubCells)
                cell.IsParentVisible = IsVisible;
        if (CurrentCell is HwndHost hwndHost) hwndHost.IsWindowVisible = IsVisible;
        else _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
    }
    [Property(OverrideKeyword = true, OnChanged = nameof(OnHoverEffectChanegd))]
    bool _HoverEffect;
    void OnHoverEffectChanegd()
    {
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoverEffect)));
    }

    [Property(OverrideKeyword = true, OnChanged = nameof(OnOrientationChanged))]
    Orientation _Orientation;

    void OnOrientationChanged()
    {
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    ICell ThisAsICell => this;
    public override bool HasWindow => CurrentCell is not null;
    public override bool HasSubCells => SubCells is not null;
    public bool HasVerticalSubCells => HasSubCells && Orientation == Orientation.Vertical;
    public bool HasHorizontalSubCells => HasSubCells && Orientation == Orientation.Horizontal;
    
    public override bool Empty => !(HasWindow || HasSubCells);
    
    public override void SplitHorizontally(int Amount)
    {
        if (!Empty) throw new InvalidOperationException();
        Orientation = Orientation.Vertical;
        SubCells = CraeteNCells(Amount);
    }
    
    public override void SplitVertically(int Amount)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();
        Orientation = Orientation.Horizontal;
        SubCells = CraeteNCells(Amount);
    }
    
    Cell[] CraeteNCells(int Amount)
    {
        return (from _ in Enumerable.Range(0, Amount) select new Cell(MainWindow, null, null, default)).ToArray();
    }
    
    public override void RegisterWindow(Window Window)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();
        CurrentCell = new(MainWindow, Window);

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