using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Windowing.Window;
using System.Collections.Generic;
using System.Linq;
using Get.EasyCSharp;

namespace UnitedSets.Tabs;

partial class WindowHostTab
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
