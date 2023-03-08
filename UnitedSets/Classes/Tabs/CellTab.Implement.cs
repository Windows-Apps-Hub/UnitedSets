using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Window = WinWrapper.Window;
using WinUIEx;
using WinUI3HwndHostPlus;
using UnitedSets.Windows;
using UnitedSets.Windows.Flyout;
using UnitedSets.Windows.Flyout.Modules;

namespace UnitedSets.Classes.Tabs;

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
                where cell.HasWindow
                select cell.CurrentCell!.HostedWindow.TryCloseAsync()
            );
            while (MainCell.AllSubCells.Any(x => x.HasWindow && x.CurrentCell!.HostedWindow.IsValid))
            {
                await Task.Delay(500);
            }
            _IsDisposed = true;
        });
        if (MainWindow.Tabs.Contains(this)) MainWindow.Tabs.Remove(this);
    }

    public override void DetachAndDispose(bool JumpToCursor = false)
    {
        //var window = new MainWindow();
        //window.Tabs.Add(new CellTab(window, MainCell.DeepClone(window)));
        foreach (var cell in MainCell.AllSubCells)
        {
			if (cell.CurrentCell is HwndHost hwndHost) {
				SaveTabData(hwndHost);
				hwndHost.DetachAndDispose();
			}
        }
        _IsDisposed = true;
        //window.Activate();
    }

    // UI
    protected override async void OnDoubleClick()
    {
        var flyout = new LeftFlyout(
            Window.FromWindowHandle(MainWindow.GetWindowHandle()),
            new BasicTabFlyoutModule(this),
            new MultiWindowModifyFlyoutModule(
                (
                    from x in MainCell.AllSubCells
                    where x.HasWindow
                    select x.CurrentCell
                ).ToArray()
            )
        );
        await flyout.ShowAsync();
        flyout.Close();
    }
}
