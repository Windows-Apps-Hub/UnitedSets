using Get.UI.Data;
using UnitedSets.Classes;
using WindowHoster;

namespace UnitedSets.UI.Controls.Cell;
public partial class WindowCellVisualizer(WindowCell cell) : TemplateControl<WindowHost>
{
    protected override void Initialize(WindowHost rootElement)
    {
        rootElement.Margin = new(10);
        rootElement.AssociatedWindow = cell.Window;
    }
}
