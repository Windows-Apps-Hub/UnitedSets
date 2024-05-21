using EasyCSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Windows.Foundation;
using WinUIEx;
using UnitedSets.Mvvm.Services;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using CommunityToolkit.Mvvm.Input;
using WinWrapper.Input;
using WinUIEx;
using UnitedSets.Tabs;

namespace UnitedSets.UI.AppWindows;

public sealed partial class FloatingTaskbar : SizeToContentWindow
{
    
    MainWindow MainWindow;
    public FloatingTaskbar(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        InitializeComponent();
        SystemBackdrop = new InfiniteSystemBackdrop<MicaController>();
        var presenter = ((OverlappedPresenter)AppWindow.Presenter);
        presenter.IsResizable = false;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.SetBorderAndTitleBar(true, false);
        RadioButtons.Resources["RadioButtonsTopHeaderMargin"] = default(Thickness);
        RadioButtons.Resources["RadioButtonsColumnSpacing"] = default(double);
        RadioButtons.Resources["RadioButtonsRowSpacing"] = default(double);
        Minimize.Resources["ChromeButtonSymbol"] = (Symbol)0xe921;
        Minimize.Click += (_, _) => mainWindow.Minimize();
        Maximize.Resources["ChromeButtonSymbol"] = (Symbol)0xe922; // restore: 0xe923
        Maximize.Click += (_, _) => mainWindow.Maximize();
        CloseBtn.Resources["ChromeButtonSymbol"] = (Symbol)0xe8bb;
        MoveSymbol.Symbol = (Symbol)0xe7c2;
        MoveButton.PointerMoved += MoveButton_PointerMoved;
        //Close.Click += async (_, _) => await mainWindow.RequestCloseAsync(MainWindow.CloseMode.ReleaseWindow);
    }

    //private void MoveButton_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    //{
    //    var ev = e.GetCurrentPoint(Content);
    //    if (ev.IsInContact)
    //    {
    //        var newPoint = ev.Position;
    //        var scale = HwndExtensions.GetDpiForWindow(this.GetWindowHandle()) / 96f;
    //        var position = MainWindow.AppWindow.Position;
    //        position.X += (int)((newPoint.X - OriginalPoint.X) * scale);
    //        position.Y += (int)((newPoint.Y - OriginalPoint.Y) * scale);
    //        MainWindow.AppWindow.Move(position);
    //    }
    //}

    //Point OriginalPoint;
    //private void MoveButton_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    //{
    //    OriginalPoint = e.GetCurrentPoint(Content).Position;
    //}
    private void MoveButton_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var ev = e.GetCurrentPoint(Content);
        if (ev.IsInContact)
        {
            var newPoint = Cursor.Position; //ev.Position;
            var point = MainWindow.AppWindow.Position;
            point.X += newPoint.X - prevPoint.X;
            point.Y += newPoint.Y - prevPoint.Y;
            prevPoint = newPoint;
            MainWindow.AppWindow.Move(point);
        } else
        {
            prevPoint = Cursor.Position;
        }
    }

    System.Drawing.Point prevPoint;
    [RelayCommand]
    void CloseFlyout()
    {
        CloseWindowFlyout.Hide();
    }
    [RelayCommand]
    async void ExitCloseAllWindows()
    {
        CloseWindowFlyout.Hide();
        await MainWindow.RequestCloseAsync(MainWindow.CloseMode.CloseWindow);
    }
    [RelayCommand]
    async void ExitRelaseAllWindows()
    {
        CloseWindowFlyout.Hide();
        await MainWindow.RequestCloseAsync(MainWindow.CloseMode.ReleaseWindow);
    }
    private void RadioButton_DragOver(object sender, DragEventArgs e)
    {
        MainWindow.OnDragOverTabViewItem(sender);
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rbtn && rbtn.Tag is TabBase tb)
            UnitedSetsApp.Current.SelectedTab = tb;
    }
}
