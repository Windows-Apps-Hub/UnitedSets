using System.Diagnostics;
using UnitedSets.UI.Controls.Cells.Resizer;

namespace UnitedSets.UI.Controls;

public partial class VerticalTabsResizer : CustomSizerBase
{
    public VerticalTabsResizer()
    {
        Orientation = Orientation.Horizontal;
    }
    public double Minimum { get; set; } = 10;
    public double Maximum { get; set; } = double.PositiveInfinity;
    OrientedStack? panel;
    UIElement? target;
    double _targetInitSize;
    protected override void OnDragStarting()
    {
        panel = this.FindAscendant<OrientedStack>();
        target = panel?.Children[0];
        var targetsize = (target?.ActualSize ?? default);
        _targetInitSize = target is null ? default : OrientedStack.LengthValueProperty.GetValue(target);
    }

    protected override bool OnDragHorizontal(double horizontalChange)
    {
        if (panel is null || target is null) return false;
        if (panel.Orientation != Orientation.Horizontal) return false;
        CommonOnDrag(horizontalChange);
        return true;
    }


    protected override bool OnDragVertical(double verticalChange) => false;
    void CommonOnDrag(double change)
    {
        if (panel is null || target is null) return;
        var targetNewSize = Math.Clamp(_targetInitSize + change, Minimum, Maximum);
        Debug.WriteLine($"_targetInitSize = {_targetInitSize}; targetNewSize = {targetNewSize}");
        OrientedStack.LengthValueProperty.SetValue(target, targetNewSize);
    }
}
