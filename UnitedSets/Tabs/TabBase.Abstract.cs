using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Get.EasyCSharp;
using Microsoft.UI.Xaml;
using Window = WinWrapper.Windowing.Window;

namespace UnitedSets.Tabs;

partial class TabBase
{   
    protected abstract Bitmap? BitmapIcon { get; }
    public abstract BitmapImage? Icon { get; }
    public abstract string DefaultTitle { get; }

    public abstract IEnumerable<Window> Windows { get; }
    //public abstract bool Selected { get; set; }
    public abstract bool IsDisposed { get; }
    
    public abstract void DetachAndDispose(bool JumpToCursor = false);
    ///// <summary>
    ///// Release the ownership of the window without detaching or disposing it.
    ///// </summary>
    //public abstract void ReleaseOwnership(Window window);
    public abstract Task TryCloseAsync();
    
    [Event(typeof(PointerEventHandler), Name = "TabClickEv")]
    public abstract void Focus();

    public virtual void UpdateStatusLoop() { }

    [Event(typeof(DoubleTappedEventHandler), Name = "TabDoubleTapped", Visibility = GeneratorVisibility.Public)]
    protected virtual void OnDoubleClick([CastFrom(typeof(object))] UIElement sender, DoubleTappedRoutedEventArgs args) { }
    [Event(typeof(RightTappedEventHandler), Name = "TabRightTapped", Visibility = GeneratorVisibility.Public)]
    protected virtual void OnRightClick([CastFrom(typeof(object))] UIElement sender, RightTappedRoutedEventArgs args) { }
}
