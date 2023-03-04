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
namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    readonly MainWindow MainWindow;
    public readonly WindowEx Window;
    public event Action Closed;

    public HwndHost HwndHost { get; }

    [Property(OverrideKeyword = true, OnChanged = nameof(OnSelectedChanged))]
    bool _Selected;
    void OnSelectedChanged()
    {
        HwndHost.IsWindowVisible = _Selected;
        if (_Selected) HwndHost.FocusWindow();
        InvokePropertyChanged(nameof(Selected));
    }

    
}
