#nullable enable
using CommunityToolkit.Labs.WinUI;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.WinUI.UI;
namespace Get.XAMLTools;

public class EzRelativeStackPanelResizer : SizerBase
{
    EzRelativeStackPanel? panel;
    UIElement? target;
    FrameworkElement? sibling;
    double _targetInitSize, _targetInitRS, _siblingInitRS;
    double _panelSize;
    double _ratioSize;
    protected override void OnDragStarting()
    {
        panel = this.FindAscendant<EzRelativeStackPanel>();
        target = panel?.Children.FirstOrDefault(x => x.FindDescendant<EzRelativeStackPanelResizer>(x => x == this) is not null);
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
        _targetInitRS = target is null ? default : EzRelativeStackPanel.GetRelativeSize(target);
        _siblingInitRS = sibling is null ? default : EzRelativeStackPanel.GetRelativeSize(sibling);
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
        EzRelativeStackPanel.SetRelativeSize(target, targetNewRS);
        EzRelativeStackPanel.SetRelativeSize(sibling, siblingNewRS);
    }
}