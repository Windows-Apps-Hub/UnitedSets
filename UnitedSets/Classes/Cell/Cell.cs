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
using System.Diagnostics;
using System.IO;
using Windows.Storage.Streams;
using WinWrapper;

namespace UnitedSets.Classes;
public partial class Cell : ICell, INotifyPropertyChanged
{
    public MainWindow MainWindow { get; }
    public Cell(MainWindow MainWindow, HwndHost? CurrentCell, Cell[]? SubCells, Orientation Orientation)
    {
        this.MainWindow = MainWindow;
        _CurrentCell = CurrentCell;
        _SubCells = SubCells;
        _Orientation = Orientation;
    }
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
    public bool IsVisible
    {
        get => IsParentVisible && _IsVisible;
        set
        {
            _IsVisible = value;
            if (_SubCells is not null)
                foreach (var cell in _SubCells)
                    cell.IsParentVisible = value;
            OnVisibleChanged();
        }
    }
    void OnVisibleChanged()
    {
        if (CurrentCell is HwndHost hwndHost) hwndHost.IsWindowVisible = IsVisible;
        else _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
    }
    bool _HoverEffect;
    public bool HoverEffect
    {
        get => _HoverEffect;
        set
        {
            if (_HoverEffect == value) return;
            _HoverEffect = value;
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoverEffect)));
        }
    }
    public HwndHost? CurrentCell
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



    public Cell[]? SubCells
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



    public Orientation Orientation
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
    public bool HasWindow => CurrentCell is not null;
    public bool HasSubCells => SubCells is not null;
    public bool HasVerticalSubCells => HasSubCells && Orientation == Orientation.Vertical;
    public bool HasHorizontalSubCells => HasSubCells && Orientation == Orientation.Horizontal;
    public bool Empty => !(HasWindow || HasSubCells);
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
        return (from _ in Enumerable.Range(0, Amount) select new Cell(MainWindow, null, null, default)).ToArray();
    }
    public void RegisterWindow(Window Window)
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
public interface ICell : INotifyPropertyChanged
{
    MainWindow MainWindow { get; }
    HwndHost? CurrentCell { get; set; }
    Cell[]? SubCells { get; set; }
    bool IsVisible { get; }

    Orientation Orientation { get; set; }

    public bool HasWindow { get; }
    public bool HasSubCells { get; }
    public bool Empty { get; }
    public bool HoverEffect { get; }

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

    void SplitHorizontally(int Amount);
    void SplitHorizontallyClickEv(object o, RoutedEventArgs _2)
    {
        //if (Keyboard.IsShiftDown)
        //{
        //    var nb = new NumberBox { Value = 2 };
        //    if (await new ContentDialog
        //    {
        //        Title = "Enter The Number of Splits",
        //        XamlRoot = ((UIElement)o).XamlRoot,
        //        Content = nb,
        //        PrimaryButtonText = "Okay",
        //        SecondaryButtonText = "Cancel"
        //    }.ShowAsync() == ContentDialogResult.Primary)
        //        SplitHorizontally((int)nb.Value);
        //} else
        //{
        SplitHorizontally(2);
        //}
    }

    void SplitVertically(int Amount);
    void SplitVerticallyClickEv(object o, RoutedEventArgs _2)
    {
        //if (Keyboard.IsShiftDown)
        //{
        //    var nb = new NumberBox { Value = 2 };
        //    if (await new ContentDialog
        //    {
        //        Title = "Enter The Number of Splits",
        //        XamlRoot = ((UIElement)o).XamlRoot,
        //        Content = nb,
        //        PrimaryButtonText = "Okay",
        //        SecondaryButtonText = "Cancel"
        //    }.ShowAsync() == ContentDialogResult.Primary)
        //        SplitVertically((int)nb.Value);
        //}
        //else
        //{
        SplitVertically(2);
        //}
    }

    void RegisterWindow(Window Window);

    void DragOverEv(object sender, DragEventArgs e)
    {
        // Error https://github.com/microsoft/microsoft-ui-xaml/issues/7002
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!Empty) return;
        DataPackageView dataview = e.DataView;
        var formats = dataview.AvailableFormats.ToList();
        if (formats.Contains("UnitedSetsTabWindow"))
            e.AcceptedOperation = DataPackageOperation.Move;
    }

    async void DropEv(object sender, DragEventArgs e)
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
    public void LoadEv(object sender, RoutedEventArgs e)
    {
        // ThePresentor does not update the selector when property changed is fired
        PropertyChanged += delegate
        {
            if (sender is Grid CP)
            {
                // Invalidate Content Template Selector
                //var t = CP.ContentTemplateSelector;
                //CP.ContentTemplateSelector = null;
                //CP.ContentTemplateSelector = t;
            }
        };
    }
}