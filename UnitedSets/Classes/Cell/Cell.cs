using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using System.Linq;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Window = WinWrapper.Window;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.Storage.Streams;
using WinWrapper;

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


    HwndHost? _CurrentCell;

    Cell[]? _SubCells;

    Orientation _Orientation;
    bool _IsParentVisible = true;
    bool IsParentVisible
    {
        get => _IsParentVisible;
        set
        {
            _IsParentVisible = value;
            OnVisibleChanged();
        }
    }
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
    bool _HoverEffect;
    public override bool HoverEffect
    {
        get => _HoverEffect;
        set
        {
            if (_HoverEffect == value) return;
            _HoverEffect = value;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoverEffect)));
        }
    }
    public override HwndHost? CurrentCell
    {
        get
        {
            return _CurrentCell;
        }

        set
        {
            _CurrentCell = value;
            if (value is not null)
            {
                value.Closed += delegate
                {
                    CurrentCell = null;
                };
                value.IsWindowVisible = IsVisible;
            }
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }



    public override Cell[]? SubCells
    {
        get
        {
            return _SubCells;
        }

        set
        {
            _SubCells = value;
            if (value is not null)
                foreach (var cell in value)
                    cell.IsVisible = IsVisible;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }



    public override Orientation Orientation
    {
        get
        {
            return _Orientation;
        }

        set
        {
            _Orientation = value;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
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
public abstract class ICell : INotifyPropertyChanged
{
    public abstract MainWindow MainWindow { get; }
    public abstract HwndHost? CurrentCell { get; set; }
    public abstract Cell[]? SubCells { get; set; }
    public abstract bool IsVisible { get; set; }

    public abstract Orientation Orientation { get; set; }

    public abstract bool HasWindow { get; }
    public abstract bool HasSubCells { get; }
    public abstract bool Empty { get; }
    public abstract bool HoverEffect { get; set; }

    public IEnumerable<ICell> AllSubCells
    {
        get
        {
            return GetAllSubCells();
        }
    }
    private IEnumerable<ICell> GetAllSubCells()
    {
        yield return this;
        if (SubCells is null) yield break;
        foreach (var cell in SubCells)
            foreach (var cellsubcell in ((ICell)cell).AllSubCells)
                yield return cellsubcell;
    }

    public abstract void SplitHorizontally(int Amount);
    public void SplitHorizontallyClickEv(object o, RoutedEventArgs _2)
    {
        SplitHorizontally(CellAddCount);
    }
    public int CellAddCount = 2;
    public string CellAddCountAsString => CellAddCount.ToString();
    public void AddCellAddCountClickEv(object o, RoutedEventArgs _2)
    {
        CellAddCount += 1;
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCount)));
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCountAsString)));
    }
    public void RemoveCellAddCountClickEv(object o, RoutedEventArgs _2)
    {
        if (CellAddCount <= 2) return;
        CellAddCount -= 1;
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCount)));
        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellAddCountAsString)));
    }
    public abstract void SplitVertically(int Amount);
    public void SplitVerticallyClickEv(object o, RoutedEventArgs _2)
    {
        SplitVertically(CellAddCount);
    }

    public abstract void RegisterWindow(Window Window);
    public void DragOverEv(object sender, DragEventArgs e)
    {
        // Error https://github.com/microsoft/microsoft-ui-xaml/issues/7002
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) return;
        DataPackageView dataview = e.DataView;
        var formats = dataview.AvailableFormats.ToList();
        if (formats.Contains("UnitedSetsTabWindow"))
            e.AcceptedOperation = DataPackageOperation.Move;
    }

    public async void DropEv(object sender, DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) return;
        DataPackageView dataview = e.DataView;
        var formats = dataview.AvailableFormats.ToList();
        if (formats.Contains("UnitedSetsTabWindow"))
        {
            var stream = (IRandomAccessStream)await e.DataView.GetDataAsync("UnitedSetsTabWindow");
            byte[] bytes = new byte[sizeof(long)];
            await stream.AsStreamForRead().ReadAsync(bytes);
            var window = Window.FromWindowHandle((nint)BitConverter.ToInt64(bytes));
            var ret = PInvoke.SendMessage(window.Owner, MainWindow.UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
            RegisterWindow(window);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            _PropertyChanged += value;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        remove
        {
            _PropertyChanged -= value;
        }
    }
    protected PropertyChangedEventHandler? _PropertyChanged;
}