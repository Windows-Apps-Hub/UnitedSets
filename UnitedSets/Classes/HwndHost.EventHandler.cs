using Microsoft.UI.Xaml;
using System;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Window = Microsoft.UI.Xaml.Window;
using WindowEx = WinWrapper.Window;
using WinWrapper;
using Windows.Win32.UI.WindowsAndMessaging;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Graphics.Dwm;
using Windows.Foundation;
using EasyCSharp;

namespace UnitedSets.Classes;

partial class HwndHost
{
    [Event(typeof(DependencyPropertyChangedCallback))]
    void OnPropChanged() => Task.Run(OnWindowUpdate);

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowChangedEventArgs>))]
    [Event(typeof(SizeChangedEventHandler))]
    void WinUIAppWindowChanged() => Task.Run(OnWindowUpdate);
}
