using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Window = WinWrapper.Window;
using Visibility = Microsoft.UI.Xaml.Visibility;
using WinUIEx;
using EasyCSharp;
using WinUI3HwndHostPlus;
namespace UnitedSets.Classes;

public partial class CellTab : TabBase
{
    [Property(OnChanged = nameof(OnMainCellChanged))]
    public Cell _MainCell;
    void OnMainCellChanged() => InvokePropertyChanged(nameof(MainCell));

    public CellTab(MainWindow MainWindow)
        : this(MainWindow, new(MainWindow, null, null, Orientation.Horizontal))
    {
    }
    readonly MainWindow MainWindow;
    protected CellTab(MainWindow MainWindow, Cell Cell) : base(MainWindow.TabView)
    {
        this.MainWindow = MainWindow;
        _MainCell = new(MainWindow, null, null, Orientation.Horizontal);
    }

    public override BitmapImage? Icon => null;

    public override string DefaultTitle => "Cell Tab";

    public override IEnumerable<Window> Windows => Enumerable.Repeat(default(Window), 0);
    [Property(OnChanged = nameof(OnSelectedChanged), OverrideKeyword = true)]
    bool _Selected;
    void OnSelectedChanged()
    {
        _MainCell.IsVisible = _Selected;
        InvokePropertyChanged(nameof(Selected));
        InvokePropertyChanged(nameof(Visibility));
    }

    Visibility Visibility => Selected ? Visibility.Visible : Visibility.Collapsed;
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate, OverrideKeyword = true)]
    bool _IsDisposed;

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

    [Event(typeof(RoutedEventHandler))]
    public void ContentLoadEv(object sender)
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
