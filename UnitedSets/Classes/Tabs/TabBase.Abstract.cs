using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using WinWrapper;
using Windows.Win32.Graphics.Gdi;
using Microsoft.UI.Xaml.Input;
using EasyCSharp;
using Microsoft.UI.Xaml;
using Window = WinWrapper.Window;
using CommunityToolkit.Mvvm.Input;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{   
    protected abstract Bitmap? BitmapIcon { get; }
    public abstract BitmapImage? Icon { get; }
    public abstract string DefaultTitle { get; }

    public abstract IEnumerable<Window> Windows { get; }
    public abstract bool Selected { get; set; }
    public abstract bool IsDisposed { get; }
    
    public abstract void DetachAndDispose(bool JumpToCursor = false);
    public abstract Task TryCloseAsync();
    
    [Event(typeof(PointerEventHandler), Name = "TabClickEv")]
    public abstract void Focus();

    public virtual void UpdateStatusLoop() { }

    [Event(typeof(DoubleTappedEventHandler), Name = "TabDoubleTapped", Visibility = GeneratorVisibility.Public)]
    protected virtual void OnDoubleClick([CastFrom(typeof(object))] UIElement sender, DoubleTappedRoutedEventArgs args) { }
}
