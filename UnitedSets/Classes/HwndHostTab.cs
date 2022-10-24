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
using Microsoft.UI.Xaml.Media;
using UnitedSets.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using System.Linq;

namespace UnitedSets.Classes;

public class HwndHostTab : ITab, INotifyPropertyChanged
{
    // Assumption: 1 Process = 1 Window
    public static List<WindowEx> MainWindows = new();
    public static event Action? OnUpdateStatusLoopComplete;
    static SynchronizedCollection<HwndHostTab> HwndHostTabs = new();
    static HwndHostTab()
    {
        Thread UpdateStatusLoop = new(() =>
        {
            while (true)
            {
                do
                    Thread.Sleep(500);
                while (!MainWindows.Any(x => x.IsVisible));

                try
                {
                    foreach (var tab in HwndHostTabs)
                    {
                        if (tab.HwndHost.IsDisposed) HwndHostTabs.Remove(tab);
                        else tab.UpdateStatusLoop();
                    }
                    OnUpdateStatusLoopComplete?.Invoke();
                } catch
                {
                    Debug.WriteLine("[United Sets Update Status Loop] Exception Occured");
                }
            }
        })
        {
            Name = "United Sets Update Status Loop"
        };
        UpdateStatusLoop.Start();
    }

    public SettingsService Settings = App.Current.Services.GetService<SettingsService>() ?? throw new InvalidOperationException("Settings Init Failed"); // cursed
    public readonly WindowEx Window;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action Closed;
    MainWindow MainWindow;
    public HwndHost HwndHost { get; }
    public BitmapImage? Icon { get; set; }
    IntPtr _Icon = IntPtr.Zero;
    string _Title;
    public string Title => Window.TitleText;
    bool _Selected;
    public bool Selected
    {
        get => _Selected;
        set
        {
            _Selected = value;
            HwndHost.IsWindowVisible = value;
            if (value) HwndHost.FocusWindow();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
        }
    }
    
    void UpdateStatusLoop()
    {
        if (_Title != Title)
        {
            _Title = Title;
            HwndHost.DispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title))));
        }
        var icon = Window.LargeIconPtr;
        if (icon == IntPtr.Zero) icon = Window.SmallIconPtr;
        if (_Icon != icon)
        {
            _Icon = icon;
            HwndHost.DispatcherQueue.TryEnqueue(UpdateAppIcon);
        }
        
    }

    public void TabClick(object sender, PointerRoutedEventArgs e)
    {
        HwndHost.IsWindowVisible = true;
        HwndHost.FocusWindow();
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
        _Title = Title;
        UpdateAppIcon();
        HwndHostTabs.Add(this);
    }

    async void UpdateAppIcon()
    {
        var icon = Window.LargeIcon ?? Window.SmallIcon;
        if (icon is not null)
        {
            Icon = await ImageFromIcon(icon);
            icon.Dispose();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
        }
    }

    async static ValueTask<BitmapImage> ImageFromIcon(Bitmap Icon)
    {
        using var ms = new MemoryStream();
        Icon.MakeTransparent(Color.Black);
        Icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return await ImageFromStream(ms);
    }


    async static ValueTask<BitmapImage> ImageFromStream(Stream Stream)
    {
        var image = new BitmapImage();

            Stream.Seek(0, SeekOrigin.Begin);
            await image.SetSourceAsync(Stream.AsRandomAccessStream());
      

        return image;
    }
    public async Task TryCloseAsync() => await Window.TryCloseAsync();
    public void TryClose() => Window.TryClose();
    public void TabCloseRequested(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
    {
        MainWindow.TabView.SelectedItem = sender;
        if (Settings.ExitOnClose)
            _ = TryCloseAsync();
        else
            DetachAndDispose(JumpToCursor: true);
    }
    public async void DetachAndDispose(bool JumpToCursor)
    {
        var Window = this.Window;
        HwndHost.DetachAndDispose();
        PInvoke.GetCursorPos(out var CursorPos);
        if (JumpToCursor)
            Window.Location = new Point(CursorPos.X - 100, CursorPos.Y - 30);
        Window.Focus();
        Window.Redraw();
        await Task.Delay(1000).ContinueWith(_ => Window.Redraw());
    }
}