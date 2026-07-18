using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using UnitedSets.UI.FlyoutModules;
using UnitedSets.Cells.Data;

namespace UnitedSets.Tabs;

partial class CellTab
{
    protected override Bitmap? BitmapIcon => null;
    public override BitmapImage? Icon => null;
    public override string DefaultTitle => "Cell Tab";
    public override IEnumerable<WindowEx> Windows => Enumerable.Repeat(default(WindowEx), 0);

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
        => _ = DetachAndDisposeAsync(JumpToCursor);

    public override async Task DetachAndDisposeAsync(bool JumpToCursor = false)
    {
        await Task.WhenAll(
            MainCell.AllSubCells
                .OfType<WindowCell>()
                .Select(x => x.Window.DetachAsync())
        );
        _IsDisposed = true;
    }

    // UI
    protected override void OnDoubleClick(UIElement sender, DoubleTappedRoutedEventArgs args)
        => ShowFlyout(
            [
                new CellTabFlyoutModule(this),
                new MultiWindowModifyFlyoutModule(
                    (
                        from x in MainCell.AllSubCells
                        let wc = x as WindowCell
                        where wc is not null
                        select wc.Window
                    ).ToArray()
                )
            ],
            sender
        );
    protected override void OnRightClick(UIElement sender, RightTappedRoutedEventArgs args)
        => OnDoubleClick(sender, null!);
}
