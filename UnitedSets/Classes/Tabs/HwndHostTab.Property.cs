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
    public readonly WindowEx Window;

    public OurHwndHost HwndHost { get; }

    [Property(OverrideKeyword = true, OnChanged = nameof(OnSelectedChanged))]
    bool _Selected;
    void OnSelectedChanged()
    {
		HwndHost.SetVisible(_Selected);
		InvokePropertyChanged(nameof(Selected));
		
	}

    
}
