using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using PInvoke;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
namespace UnitedSets;

interface ITab
{
    IconSource? Icon { get; }
    BitmapImage? Tempicon { get; }
    string Title { get; }
    HwndHost HwndHost { get; }
    bool Selected { get; set; }
}
public class HwndHostTab : ITab, INotifyPropertyChanged
{
    public bool Selected {
        get => HwndHost.IsWindowVisible;
        set {
            HwndHost.IsWindowVisible = value;
            if (value) HwndHost.FocusWindow();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
        }
    }
    public readonly WindowEx Window;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action Closed;
    MainWindow MainWindow;
    public HwndHostTab(MainWindow Window, WindowEx WindowEx)
    {
        MainWindow = Window;
        this.Window = WindowEx;
        HwndHost = new(Window, WindowEx) { IsWindowVisible = false };
        Closed = delegate
        {
            if (MainWindow.Tabs.Contains(this)) MainWindow.Tabs.Remove(this);
        };
        HwndHost.Closed += Closed;
        HwndHost.Updating += delegate
        {
            if (_Title != Title)
            {
                _Title = Title;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        };
        _Title = Title;
        UpdateAppIcon();
    }
    public HwndHost HwndHost { get; }
    async void UpdateAppIcon()
    {
        var icon = Window.LargeIcon;
        if (icon is not null)
            Icon = await ImageFromIcon(icon);
    }
    public IconSource? Icon { get; set; }

    public BitmapImage? Tempicon { get; set; }
    string _Title;
    public string Title => Window.Text;

    async static ValueTask<ImageIconSource> ImageFromIcon(Icon Icon)
    {
        using var ms = new MemoryStream();
        Icon.Save(ms);
        return await ImageFromStream(ms);
    }


    async static ValueTask<ImageIconSource> ImageFromStream(Stream Stream)
    {
        var image = new BitmapImage();
        try
        {
            Stream.Seek(0, SeekOrigin.Begin);
            await image.SetSourceAsync(Stream.AsRandomAccessStream());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        return new ImageIconSource { ImageSource = image };
    }
    public void TabCloseRequested(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
    {
        MainWindow.TabView.SelectedItem = sender;
        Window.TryClose();
    }
    public void DetachAndDispose()
    {
        var Window = this.Window;
        HwndHost.DetachAndDispose();
        var bounds = Window.Bounds;
        var CursorPos = User32.GetCursorPos();
        bounds.X = CursorPos.x - 100;
        bounds.Y = CursorPos.y - 30;
        Window.Bounds = bounds;
        Window.Focus();
    }
}