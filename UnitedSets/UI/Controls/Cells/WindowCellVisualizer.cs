using Get.UI.Data;
using UnitedSets.Cells;
using WindowHoster;

namespace UnitedSets.UI.Controls.Cells;
public partial class WindowCellVisualizer(WindowCell cell) : TemplateControl<WindowHost>
{
    protected override void Initialize(WindowHost rootElement)
    {
        rootElement.Margin = new(10);
        rootElement.AssociatedWindow = cell.Window;
    }
}
