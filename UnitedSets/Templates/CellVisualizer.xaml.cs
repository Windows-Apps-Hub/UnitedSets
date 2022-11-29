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
using EasyCSharp;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CellVisualizer : INotifyPropertyChanged
{
    public CellVisualizer()
    {
        Resources["HorizontalSymbol"] = (Symbol)0xE76F;
        Resources["VerticalSymbol"] = (Symbol)0xE784;
        InitializeComponent();
    }

    [Property(OnBeforeChanged = nameof(OnBeforeCellChanged), OnChanged = nameof(OnCellChanged))]
    ICell? _Cell;
    void OnBeforeCellChanged()
    {
        if (_Cell is not null)
            _Cell.PropertyChanged -= OnCellChanged;
    }
    void OnCellChanged()
    {
        if (_Cell is not null)
            _Cell.PropertyChanged += OnCellChanged;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cell)));
        UpdateTemplate();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    [Event(typeof(PropertyChangedEventHandler))]
    void OnCellChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(Cell.CellAddCountAsString) or nameof(Cell.CellAddCount)))
            DispatcherQueue.TryEnqueue(() => UpdateTemplate());
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

    
}
