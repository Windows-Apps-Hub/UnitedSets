using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using SourceGenerators;
using Microsoft.UI.Xaml;
using System.Linq;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Window = WinWrapper.Window;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSets.Classes;
public partial record class Cell(HwndHost? CurrentCell, Cell[]? SubCells, Orientation Orientation) : ICell, INotifyPropertyChanged
{
    PropertyChangedEventHandler? _PropertyChanged;
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

    
    HwndHost? _CurrentCell = CurrentCell;
    
    Cell[]? _SubCells = SubCells;
    
    Orientation _Orientation = Orientation;

    public HwndHost? CurrentCell
    {
        get
        {
            return _CurrentCell;
        }

        set
        {
            _CurrentCell = value;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCell)));
        }
    }



    public Cell[]? SubCells
    {
        get
        {
            return _SubCells;
        }

        set
        {
            _SubCells = value;
            _PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(SubCells)));
        }
    }



    public Orientation Orientation
    {
        get
        {
            return _Orientation;
        }

        set
        {
            _Orientation = value;
            _PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Orientation)));
        }
    }
    ICell ThisAsICell => this;
    public bool HasWindow => CurrentCell is not null;
    public bool HasSubCells => SubCells is not null;
    public bool Empty => !(HasWindow || HasSubCells);
    public void SplitHorizontally()
    {
        if (!Empty) throw new InvalidOperationException();
    }
    public void SplitVertically()
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();
    }
    public void RegisterWindow(Window Window)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) throw new InvalidOperationException();

    }
}
interface ICell : INotifyPropertyChanged
{
    HwndHost? CurrentCell { get; set; }
    Cell[]? SubCells { get; set; }
    
    Orientation Orientation { get; set; }

    public bool HasWindow { get; }
    public bool HasSubCells { get; }
    public bool Empty { get; }

    IEnumerable<ICell> AllSubCells
    {
        get
        {
            yield return this;
            if (SubCells is null) yield break;
            foreach (var cell in SubCells)
                if (cell.SubCells is not null)
                    foreach (var cellsubcell in cell.SubCells)
                        yield return cellsubcell;
        }
    }

    void SplitHorizontally();
    void SplitHorizontallyClickEv(object _1, RoutedEventArgs _2) => SplitHorizontally();

    void SplitVertically();
    void SplitVerticallyClickEv(object _1, RoutedEventArgs _2) => SplitVertically();

    void RegisterWindow(Window Window);

    void DragOverEv(object sender, DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) return;
        if (e.DataView.AvailableFormats.Contains("UnitedSetsTabWindow"))
            e.AcceptedOperation = DataPackageOperation.Move;
    }

    async void DropEv(object sender, DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) return;
        if (e.DataView.AvailableFormats.Contains("UnitedSetsTabWindow"))
        {
            var a = (long)await e.DataView.GetDataAsync("UnitedSetsTabWindow");
            var window = Window.FromWindowHandle((nint)a);
            var ret = PInvoke.SendMessage(window.Owner, MainWindow.UnitedSetCommunicationChangeWindowOwnership, new(), new(window));
            RegisterWindow(window);
        }
    }
}