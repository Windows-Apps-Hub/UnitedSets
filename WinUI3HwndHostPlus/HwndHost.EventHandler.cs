using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System.Threading.Tasks;
using Windows.Foundation;
using EasyCSharp;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    [Event(typeof(DependencyPropertyChangedCallback))]
    void OnPropChanged() => Task.Run(OnWindowUpdate);

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowChangedEventArgs>))]
    [Event(typeof(SizeChangedEventHandler))]
    void WinUIAppWindowChanged() => Task.Run(OnWindowUpdate);
}
