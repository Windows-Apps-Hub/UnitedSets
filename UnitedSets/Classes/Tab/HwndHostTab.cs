using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Win32;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using System.Collections.Generic;
using System.Linq;
using UnitedSets.Helpers;
using static WinUIEx.WindowExtensions;
using WinWrapper;
using WinUIEx;

namespace UnitedSets.Classes;

public class HwndHostTab : TabBase
{
    public readonly WindowEx Window;
    public override IEnumerable<WindowEx> Windows => Enumerable.Repeat(Window, 1);
    readonly MainWindow MainWindow;
    public event Action Closed;
    public HwndHost HwndHost { get; }
    IntPtr _Icon = IntPtr.Zero;
    BitmapImage? _IconBmpImg;
    public override BitmapImage? Icon => _IconBmpImg;
    string _Title;
    public override string DefaultTitle => Window.TitleText;
    bool _Selected;
    public override bool Selected
    {
        get => _Selected;
        set
        {
            _Selected = value;
            HwndHost.IsWindowVisible = value;
            if (value) HwndHost.FocusWindow();
            InvokePropertyChanged(nameof(Selected));
        }
    }
    bool _IsDisposed = false;
    public override bool IsDisposed => _IsDisposed || !Window.IsValid;
    
    public HwndHostTab(MainWindow Window, WindowEx WindowEx) : base(Window.TabView)
    {
        MainWindow = Window;
        this.Window = WindowEx;
        HwndHost = new(Window, WindowEx) { IsWindowVisible = false, BorderlessWindow = Keyboard.IsAltDown };
        Closed = delegate
        {
            if (MainWindow.Tabs.Contains(this)) MainWindow.Tabs.Remove(this);
        };
        HwndHost.Closed += Closed;
        _Title = DefaultTitle;
        UpdateAppIcon();
    }

    public override void UpdateStatusLoop()
    {
        if (_Title != DefaultTitle)
        {
            _Title = DefaultTitle;
            HwndHost.DispatcherQueue.TryEnqueue(() => InvokePropertyChanged(nameof(DefaultTitle)));
            if (!string.IsNullOrWhiteSpace(CustomTitle))
                HwndHost.DispatcherQueue.TryEnqueue(() => InvokePropertyChanged(nameof(Title)));
        }
        var icon = Window.LargeIconPtr;
        if (icon == IntPtr.Zero) icon = Window.SmallIconPtr;
        if (_Icon != icon)
        {
            _Icon = icon;
            HwndHost.DispatcherQueue.TryEnqueue(UpdateAppIcon);
        }
    }
    
    async void UpdateAppIcon()
    {
        var icon = Window.LargeIcon ?? Window.SmallIcon;
        if (icon is not null)
        {
            _IconBmpImg = await ImageHelper.ImageFromBitmap(icon);
            icon.Dispose();
            InvokePropertyChanged(nameof(Icon));
        }
    }

    public override async Task TryCloseAsync() => await Window.TryCloseAsync();
    public override async void DetachAndDispose(bool JumpToCursor)
    {
        var Window = this.Window;
        HwndHost.DetachAndDispose();
        PInvoke.GetCursorPos(out var CursorPos);
        if (JumpToCursor)
            Window.Location = new Point(CursorPos.X - 100, CursorPos.Y - 30);
        Window.Focus();
        Window.Redraw();
        await Task.Delay(1000).ContinueWith(_ => Window.Redraw());
        _IsDisposed = true;
    }
    public override void Focus()
    {
        HwndHost.IsWindowVisible = true;
        HwndHost.FocusWindow();
    }
    protected override async void OnDoubleClick()
    {
        var flyout = new LeftFlyout(
            WindowEx.FromWindowHandle(MainWindow.GetWindowHandle()),
            new BasicTabFlyoutModule(this),
            new ModifyWindowFlyoutModule(HwndHost)
        );
        await flyout.ShowAsync();
        flyout.Close();
    }
}