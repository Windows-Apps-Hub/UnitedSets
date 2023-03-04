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
using Microsoft.UI.Xaml;
using EasyCSharp;
using System.Collections;

namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    public override IEnumerable<WindowEx> Windows => Enumerable.Repeat(Window, 1);
    public override string DefaultTitle => Window.TitleText;

    [Property(
        OverrideKeyword = true,
        Visibility = GeneratorVisibility.Protected,
        SetVisibility = GeneratorVisibility.DoNotGenerate
    )]
    Bitmap? _BitmapIcon;
    [Property(PropertyName = "Icon", OverrideKeyword = true, SetVisibility = GeneratorVisibility.DoNotGenerate)]
    BitmapImage? _IconBmpImg;
    
    [Property(CustomGetExpression = "_IsDisposed || !Window.IsValid", SetVisibility = GeneratorVisibility.DoNotGenerate, OverrideKeyword = true)]
    bool _IsDisposed = false;
}
