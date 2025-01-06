#nullable enable
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.WinUI.UI;
using Get.UI.Data;
using Get.Data.Properties;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
namespace UnitedSets.Controls;

public partial class OrientedStackResizer : SizerBase
{
    public OrientedStackResizer()
    {
        DefaultStyleKey = typeof(SizerBase);
    }
    OrientedStack? panel;
    UIElement? target;
    FrameworkElement? sibling;
    double _targetInitSize, _targetInitRS, _siblingInitRS;
    double _panelSize;
    double _ratioSize;
    protected override void OnDragStarting()
    {
        panel = this.FindAscendant<OrientedStack>(x => x.Tag is "Cell Container");
        target = panel?.Children.FirstOrDefault(x => x.FindDescendant<OrientedStackResizer>(x => x == this) is not null);
        var siblingIdx = panel?.Children.IndexOf(target) + 1 ?? 0;
        if (siblingIdx != 0)
        {
            sibling = siblingIdx >= (panel?.Children?.Count ?? -1) ? null : panel?.Children[siblingIdx] as FrameworkElement;
        }
        var targetsize = (target?.ActualSize ?? default);
        _targetInitSize = Orientation is Orientation.Vertical ? targetsize.Y : targetsize.X;
        var panelsize = panel?.ActualSize ?? default;
        _panelSize = Orientation is Orientation.Vertical ? panelsize.Y : panelsize.X;
        _ratioSize = _targetInitSize / _panelSize;
        _targetInitRS = target is null ? default : OrientedStack.LengthValueProperty.GetValue(target);
        _siblingInitRS = sibling is null ? default : OrientedStack.LengthValueProperty.GetValue(sibling);
    }

    protected override bool OnDragHorizontal(double horizontalChange)
    {
        if (panel is null || target is null || sibling is null) return false;
        if (panel.Orientation != Orientation.Horizontal) return false;
        CommonOnDrag(horizontalChange);
        return true;
    }


    protected override bool OnDragVertical(double verticalChange)
    {
        if (panel is null || target is null || sibling is null) return false;
        if (panel.Orientation != Orientation.Vertical) return false;
        CommonOnDrag(verticalChange);
        return true;
    }
    void CommonOnDrag(double change)
    {
        if (panel is null || target is null || sibling is null) return;
        var targetNewSize = Math.Max(_targetInitSize + change, 10);
        var targetNewRS = _targetInitRS * targetNewSize / _targetInitSize;
        var siblingNewRS = _siblingInitRS - (targetNewRS - _targetInitRS);
        OrientedStack.LengthValueProperty.SetValue(target, targetNewRS);
        OrientedStack.LengthValueProperty.SetValue(sibling, siblingNewRS);
    }
}
