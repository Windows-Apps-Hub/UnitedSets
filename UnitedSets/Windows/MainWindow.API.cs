using EasyCSharp;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using WinRT.Interop;
using WinUIEx;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using System;
using WindowRelative = WinWrapper.WindowRelative;
using WindowEx = WinWrapper.Window;
using Cursor = WinWrapper.Cursor;
using Keyboard = WinWrapper.Keyboard;
using UnitedSets.Classes;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using UnitedSets.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using WinUIEx.Messaging;
using Microsoft.UI.Dispatching;
using System.Threading;
using System.IO;
using WinWrapper;
using System.Text.RegularExpressions;
using Windows.Foundation;
using WinUI3HwndHostPlus;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.Windows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    void AddTab(WindowEx newWindow, int? index = null)
    {
        if (!newWindow.IsValid)
            return;
        newWindow = newWindow.Root;
        if (newWindow.Handle == IntPtr.Zero)
            return;
        if (newWindow.Handle == AddTabFlyout.GetWindowHandle())
            return;
        if (newWindow.Handle == WindowEx.Handle)
            return;
        if (HwndHost.ShouldBeBlacklisted(newWindow))
            return;
        // Check if United Sets has owner (United Sets in United Sets)
        if (WindowEx.Root.Children.Any(x => x == newWindow))
            return;
        if (Tabs.Any(x => x.Windows.Any(y => y == newWindow)))
            return;
        var newTab = new HwndHostTab(this, newWindow);
        if (index.HasValue)
            Tabs.Insert(index.Value, newTab);
        else
            Tabs.Add(newTab);
        TabView.SelectedItem = newTab;
    }
}
