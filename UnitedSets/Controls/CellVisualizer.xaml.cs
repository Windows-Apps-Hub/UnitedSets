using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using UnitedSets.Classes;
using EasyCSharp;
using EasyXAMLTools;

namespace UnitedSets.Controls;

[DependencyProperty(
    typeof(Cell),
    "Cell",
    UseNullableReferenceType = true,
    GenerateLocalOnPropertyChangedMethod = true
)]
public sealed partial class CellVisualizer
{
    public CellVisualizer()
    {
        Resources["HorizontalSymbol"] = (Symbol)0xE76F;
        Resources["VerticalSymbol"] = (Symbol)0xE784;
        InitializeComponent();
    }

    partial void OnCellChanged(Cell? oldValue, Cell? newValue)
    {
        if (oldValue != null) oldValue.PropertyChanged -= OnCellPropertyChanged!;
        if (newValue != null) newValue.PropertyChanged += OnCellPropertyChanged!;
        UpdateTemplate();
    }
    
    [Event(typeof(PropertyChangedEventHandler))]
    void OnCellPropertyChanged(PropertyChangedEventArgs e)
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
