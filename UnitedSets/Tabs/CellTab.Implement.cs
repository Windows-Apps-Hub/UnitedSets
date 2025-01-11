using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Window = WinWrapper.Windowing.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using UnitedSets.UI.FlyoutModules;
using UnitedSets.Cells;
using Get.EasyCSharp;

namespace UnitedSets.Tabs;

partial class CellTab
{
    protected override Bitmap? BitmapIcon => null;
    public override BitmapImage? Icon => null;
    public override string DefaultTitle => "Cell Tab";
    public override IEnumerable<Window> Windows => Enumerable.Repeat(default(Window), 0);

    // API
    public override void Focus() { }

    public async override Task TryCloseAsync()
    {
        await Task.Run(async delegate
        {
            var allcells = MainCell.AllSubCells.ToArray();
            await Task.WhenAll(
                from cell in allcells
                let wc = cell as WindowCell
                where wc != null
                select wc.Window.Window.TryCloseAsync()
            );
            while (MainCell.AllSubCells.Any(x => x is WindowCell wc && wc.Window.IsValid))
            {
                await Task.Delay(500);
            }
            _IsDisposed = true;
        });
        DoRemoveTab();
    }

    public override void DetachAndDispose(bool JumpToCursor = false)
    {
        foreach (var cell in MainCell.AllSubCells.ToArray())
        {
            if (cell is WindowCell wc)
                wc.Window.Detach();
        }
        _IsDisposed = true;
    }

    // UI
    protected override void OnDoubleClick(UIElement sender, DoubleTappedRoutedEventArgs args)
        => ShowFlyout(
            new MultiWindowModifyFlyoutModule(
                (
                    from x in MainCell.AllSubCells
                    let wc = x as WindowCell
                    where wc is not null
                    select wc.Window
                ).ToArray()
            ),
            sender
        );
    protected override void OnRightClick(UIElement sender, RightTappedRoutedEventArgs args)
        => OnDoubleClick(sender, null!);
}
