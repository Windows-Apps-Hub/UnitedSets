using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinWrapper;
using Window = WinWrapper.Window;
using Visibility = Microsoft.UI.Xaml.Visibility;
using WinUIEx;

namespace UnitedSets.Classes;

public partial class CellTab : TabBase
{
    public Cell _MainCell;
    public Cell MainCell
    {
        get
        {
            return _MainCell;
        }

        set
        {
            _MainCell = value;
            InvokePropertyChanged(nameof(MainCell));
        }
    }
    public CellTab(MainWindow MainWindow)
        : this(MainWindow, new(MainWindow, null, null, Orientation.Horizontal))
    {
    }
    MainWindow MainWindow;
    protected CellTab(MainWindow MainWindow, Cell Cell) : base(MainWindow.TabView)
    {
        this.MainWindow = MainWindow;
        _MainCell = new(MainWindow, null, null, Orientation.Horizontal);
    }

    public override BitmapImage? Icon => null;

    public override string DefaultTitle => "Cell Tab";

    public override IEnumerable<Window> Windows => Enumerable.Repeat(default(Window), 0);

    bool _Selected;
    public override bool Selected
    {
        get => _Selected;
        set
        {
            _Selected = value;
            _MainCell.IsVisible = value;
            //foreach (var cell in ((ICell)_MainCell).IsVisible)
            //{
            //    if (cell.HasWindow) cell.CurrentCell!.IsWindowVisible = value;
            //}
            //HwndHost.IsWindowVisible = value;
            //if (value) HwndHost.FocusWindow();
            InvokePropertyChanged(nameof(Selected));
            InvokePropertyChanged(nameof(Visibility));
        }
    }
    Visibility Visibility => Selected ? Visibility.Visible : Visibility.Collapsed;
    bool _IsDisposed;
    public override bool IsDisposed => _IsDisposed;

    public override void DetachAndDispose(bool JumpToCursor = false)
    {

        //var window = new MainWindow();
        //window.Tabs.Add(new CellTab(window, MainCell.DeepClone(window)));
        foreach (var cell in MainCell.AllSubCells)
        {
            if (cell.CurrentCell is HwndHost hwndHost) hwndHost.DetachAndDispose();
        }
        _IsDisposed = true;
        //window.Activate();
    }

    public void ContentLoadEv(object sender, RoutedEventArgs e)
    {
        // ContentPresentor does not update the selector when property changed is fired
        PropertyChanged += delegate
        {
            if (sender is ContentPresenter CP)
            {
                // Invalidate Content Template Selector
                var t = CP.ContentTemplateSelector;
                CP.ContentTemplateSelector = null;
                CP.ContentTemplateSelector = t;
                CP.Visibility = Visibility;
            }
        };
    }

    public override void Focus()
    {

    }
    

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
    protected override async void OnDoubleClick()
    {
        var flyout = new TabPropertiesFlyout(
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
