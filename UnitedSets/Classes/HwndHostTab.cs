using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Windows.Win32;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using UnitedSets.Interfaces;

namespace UnitedSets.Classes;

public class HwndHostTab : ITab, INotifyPropertyChanged
{
    public readonly WindowEx Window;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action Closed;
    MainWindow MainWindow;
    public HwndHost HwndHost { get; }
    public IconSource? Icon { get; set; }
    public BitmapImage? Tempicon { get; set; }
    string _Title;
    public string Title => Window.TitleText;
    public bool Selected
    {
        get => HwndHost.IsWindowVisible;
        set
        {
            HwndHost.IsWindowVisible = value;
            if (value) HwndHost.FocusWindow();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
        }
    }


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

    async void UpdateAppIcon()
    {
        var icon = Window.LargeIcon;
        if (icon is not null)
            Icon = await ImageFromIcon(icon);
    }

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
        PInvoke.GetCursorPos(out var CursorPos);
        Window.Location = new Point(CursorPos.X - 100, CursorPos.Y - 30);
        Window.Focus();
    }
}