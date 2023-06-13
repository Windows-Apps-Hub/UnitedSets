using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using OutOfBoundsFlyout;
using System;
using System.Linq;
using TransparentWinUIWindowLib;
using UnitedSets.Classes;
using UnitedSets.Classes.Tabs;
using UnitedSets.Helpers;
using WinRT.Interop;
using WinUIEx.Messaging;
using Window = WinWrapper.Windowing.Window;
using EasyCSharp;
using Microsoft.UI.Windowing;
using Keyboard = WinWrapper.Input.Keyboard;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Foundation;
using System.Runtime.CompilerServices;
using System.Drawing;
using UnitedSets.Classes.Settings;
using UnitedSets.Mvvm.Services;
using WinUIEx;
using Microsoft.UI.Xaml.Controls;

namespace UnitedSets.UI.AppWindows;

partial class MainWindow
{
    void SetupTaskbarMode()
    {
        TabViewBorder.Visibility = Visibility.Collapsed;
        //CustomDragRegionUpdator
        IsMinimizable = false;
        IsMaximizable = false;
        Grid.SetRow(MainAreaBorder, 0);
        Grid.SetRowSpan(MainAreaBorder, 2);
        MainAreaBorder.Margin = new(0, top: 8, 0, 0);
        var window = new FloatingTaskbar(this)
        {
            IsShownInSwitchers = false,
            IsTitleBarVisible = false
        };
        var windownative = Window.FromWindowHandle(window.GetWindowHandle());
        windownative.Owner = Win32Window;
        var presenter = (OverlappedPresenter)AppWindow.Presenter;
        //ExtendsContentIntoTitleBar = false;
        //presenter.SetBorderAndTitleBar(true, false);

        AppWindow.Changed += PositionAppWindowChanged;
        void PositionAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPositionChange || args.DidSizeChange)
            {
                var bounds = Win32Window.Bounds;
                window.AnchorPoint = new(bounds.Right, bounds.Bottom + 10);
            }
        }
        window.Activate();
    }
}
