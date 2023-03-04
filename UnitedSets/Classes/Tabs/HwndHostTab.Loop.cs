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

namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    public override void UpdateStatusLoop()
    {
        if (_Title != DefaultTitle)
        {
            _Title = DefaultTitle;
            HwndHost.DispatcherQueue.TryEnqueue(() => InvokePropertyChanged(nameof(DefaultTitle)));
            if (string.IsNullOrWhiteSpace(CustomTitle))
                HwndHost.DispatcherQueue.TryEnqueue(() => TitleChanged());
        }
        var icon = Window.LargeIconPtr;
        if (icon == IntPtr.Zero) icon = Window.SmallIconPtr;
        if (_Icon != icon)
        {
            _Icon = icon;
            HwndHost.DispatcherQueue.TryEnqueue(UpdateAppIcon);
        }
    }
}
