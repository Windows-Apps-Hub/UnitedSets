using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnitedSets.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CellVisualizer : INotifyPropertyChanged
{
    ICell? _Cell;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICell? Cell
    {
        get => _Cell;
        set
        {
            if (_Cell is not null)
                _Cell.PropertyChanged -= CellChanged;
            _Cell = value;
            if (value is not null)
                value.PropertyChanged += CellChanged;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cell)));
            UpdateTemplate();
        }
    }

    private void CellChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(Cell.CellAddCountAsString) or nameof(Cell.CellAddCount)))
            DispatcherQueue.TryEnqueue(() => UpdateTemplate());
        //else
        //{
        //    if (Cell is not null)
        //    {
        //        if (Cell.HoverEffect)
        //            DispatcherQueue.TryEnqueue(() => BG.Color = (Windows.UI.Color)Resources["LayerFillColorDefault"]);
        //        else
        //            DispatcherQueue.TryEnqueue(() => BG.Color = Colors.Transparent);
        //    }
        //}
    }
    void UpdateTemplate()
    {
        ContentTemplate = Cell switch
        {
            { Empty: true, IsVisible: false } => InvisibleCellDataTemplate,
            { HasWindow: true } => WindowCellDataTemplate,
            { Empty: true, HoverEffect: false } => EmptyCellDataTemplate,
            { Empty: true, HoverEffect: true } => EmptyCellDataTemplateWindowHover,
            { HasSubCells: true, Orientation: Orientation.Vertical } => VerticalCellDataTemplate,
            { HasSubCells: true, Orientation: Orientation.Horizontal } => HorizontalCellDataTemplate,
            _ => null
        };
    }

    public CellVisualizer()
    {
        Resources["HorizontalSymbol"] = (Symbol)0xE76F;
        Resources["VerticalSymbol"] = (Symbol)0xE784;
        InitializeComponent();
    }
}
