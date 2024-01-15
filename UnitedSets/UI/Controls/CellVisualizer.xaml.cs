using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using UnitedSets.Classes;
using Get.EasyCSharp;
using Get.XAMLTools;
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
        if (oldValue != null) oldValue.PropertyChanged -= OnCellPropertyChanged;
        if (newValue != null) newValue.PropertyChanged += OnCellPropertyChanged;
        UpdateTemplate();
    }
    
    [Event(typeof(PropertyChangedEventHandler))]
    void OnCellPropertyChanged(PropertyChangedEventArgs? e)
    {
        if (e is null) return;
        if (e.PropertyName is not nameof(Cell.CellAddCount))
            DispatcherQueue.TryEnqueue(() => UpdateTemplate());
    }
    void UpdateTemplate()
    {
        ContentTemplate = Cell switch
        {
            { ContainsWindow: true } => WindowCellDataTemplate,
            { IsEmpty: true, HoverEffect: false } => EmptyCellDataTemplate,
            { IsEmpty: true, HoverEffect: true } => EmptyCellDataTemplateWindowHover,
            { ContainsSubCells: true, Orientation: Orientation.Vertical } => VerticalCellDataTemplate,
            { ContainsSubCells: true, Orientation: Orientation.Horizontal } => HorizontalCellDataTemplate,
            _ => null
        };
    }

    
}
