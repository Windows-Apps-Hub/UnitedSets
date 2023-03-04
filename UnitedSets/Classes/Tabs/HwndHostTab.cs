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
using WinUI3HwndHostPlus;
using UnitedSets.Windows;
using UnitedSets.Windows.Flyout;
using UnitedSets.Windows.Flyout.Modules;
using Windows.Win32.Graphics.Gdi;
using EasyCSharp;
using Microsoft.UI.Xaml.Controls;

namespace UnitedSets.Classes.Tabs;

public partial class HwndHostTab : TabBase
{
    IntPtr _Icon = IntPtr.Zero;
    string _Title;
    
    public HwndHostTab(MainWindow Window, WindowEx WindowEx, bool IsTabSwitcherVisibile) : base(Window.TabView, IsTabSwitcherVisibile)
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

    async void UpdateAppIcon()
    {
        var icon = Window.LargeIcon ?? Window.SmallIcon;
        if (icon is not null)
        {
            var oldIcon = _BitmapIcon;
            _IconBmpImg = await ImageHelper.ImageFromBitmap(icon);
            _BitmapIcon = icon;
            OnIconChanged();
            oldIcon?.Dispose();
        }
    }
}