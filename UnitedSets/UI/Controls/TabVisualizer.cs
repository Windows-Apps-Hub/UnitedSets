using Get.Data.Bindings.Linq;
using Get.UI.Data;
using Get.XAMLTools;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Cells;
using UnitedSets.Configurations;
using UnitedSets.Tabs;
using UnitedSets.UI.Controls.Cells;
using WindowHoster;

namespace UnitedSets.UI.Controls;
[DependencyProperty<TabBase>("Tab", UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
public partial class TabVisualizer : TemplateControl<Grid>
{
    Grid? rootElement;
    protected override void Initialize(Grid rootElement)
    {
        this.rootElement = rootElement;
        OnTabChanged(null, Tab);
    }
    partial void OnTabChanged(TabBase? oldValue, TabBase? newValue)
    {
        if (rootElement is null) return;
        rootElement.Children.Clear();
        if (newValue != null)
            rootElement.Children.Add(
                newValue switch
                {
                    CellTab ct => new GenericCellVisualizer(ct.MainCell)
                    {
                        CellBinding = OneWay(ct.MainCellProperty.Select(x => (Cell)x))
                    },
                    WindowHostTab wt => new WindowHost { AssociatedWindow = wt.RegisteredWindow },
                    _ => throw new System.InvalidCastException("Unknown tab type")
                }
            );
    }
}
