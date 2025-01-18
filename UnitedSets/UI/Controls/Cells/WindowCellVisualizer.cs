using Get.Data.Properties;
using Get.UI.Data;
using UnitedSets.Cells;
using WindowHoster;

namespace UnitedSets.UI.Controls.Cells;
[AutoProperty]
public partial class WindowCellVisualizer(WindowCell cell) : TemplateControl<WindowHost>
{
    public IProperty<double> CellMarginProperty { get; } = Auto(10d);
    protected override void Initialize(WindowHost rootElement)
    {
        CellMarginProperty.ApplyAndRegisterForNewValue((_, x) =>
        {
            rootElement.Margin = new(x);
        });
        rootElement.AssociatedWindow = cell.Window;
    }
}
