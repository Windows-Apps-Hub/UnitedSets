using EasyCSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitedSets.Services;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using WinWrapper;
using Window = WinWrapper.Window;

namespace UnitedSets.Classes.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
    public readonly static List<Window> MainWindows = new();
    public static event Action? OnUpdateStatusLoopComplete;
    static readonly SynchronizedCollection<TabBase> AllTabs = new();
    static readonly WindowClass UnitedSetsSwitcherWindowClass;
    static TabBase()
    {
        //UnitedSetsSwitcherWindowClass = new WindowClass(nameof(UnitedSetsSwitcherWindowClass),
        //    (hwnd, msg, wparam, lparam) =>
        //    {
        //        if (msg == PInvoke.WM_ACTIVATE)
        //        {
        //            var tab = AllTabs.FirstOrDefault(x => x.Windows.FirstOrDefault(x => x.Handle == hwnd, default) != default);
        //            tab?.SwitcherWindowFocusCallback();
        //        }
        //        return PInvoke.DefWindowProc(hwnd, msg, wparam, lparam);
        //    },
        //    ClassStyle: WNDCLASS_STYLES.CS_VREDRAW | WNDCLASS_STYLES.CS_HREDRAW,
        //    BackgroundBrush: new(PInvoke.GetStockObject(GET_STOCK_OBJECT_FLAGS.BLACK_BRUSH).Value));
        Thread UpdateStatusLoop = new(() =>
        {
            while (true)
            {
                do
                    Thread.Sleep(500);
                while (!MainWindows.Any(x => x.IsVisible));

                try
                {
                    foreach (var tab in AllTabs)
                    {
                        if (tab.IsDisposed) AllTabs.Remove(tab);
                        else tab.UpdateStatusLoop();
                    }
                    OnUpdateStatusLoopComplete?.Invoke();
                }
                catch
                {
                    Debug.WriteLine("[United Sets Update Status Loop] Exception Occured");
                }
            }
        })
        {
            Name = "United Sets Update Status Loop"
        };
        UpdateStatusLoop.Start();
    }
    static readonly SettingsService Settings
        = App.Current.Services.GetService<SettingsService>() ?? throw new InvalidOperationException("Settings Init Failed");

    public TabBase(TabView Parent, bool IsSwitcherVisible)
    {
        AllTabs.Add(this);
        ParentTabView = Parent;
        //var thread = new Thread(() =>
        //{
        //    SwitcherWindow = Window.CreateNewWindow("EZ", UnitedSetsSwitcherWindowClass);
        //    SwitcherWindow.IsVisible = true;
        //    SwitcherWindow.Show();
        //    SwitcherWindow.Style = WINDOW_STYLE.WS_VISIBLE;

        //    SwitcherWindow.ExStyle = WINDOW_EX_STYLE.WS_EX_TRANSPARENT | WINDOW_EX_STYLE.WS_EX_LAYERED;
        //    WinWrapper.Application.RunMessageLoopOnCurrentThread();
        //})
        //{ Name = "Window Thread" };
        //thread.SetApartmentState(ApartmentState.STA);
        //thread.Start();
        //IsSwitcherVisible = true;
        //if (!IsSwitcherVisible) SwitcherWindow.ExStyle |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;

    }

    Window SwitcherWindow;
    public bool IsSwitcherVisible { get; }
    public TabView ParentTabView { get; }
    protected abstract HBITMAP? NativeIcon { get; }
    public abstract BitmapImage? Icon { get; }
    public abstract string DefaultTitle { get; }

    public string Title => string.IsNullOrWhiteSpace(CustomTitle) ? DefaultTitle : CustomTitle;

    [Property(OnChanged = nameof(OnCustomTitleChanged))]
    string _CustomTitle = "";
    void OnCustomTitleChanged()
    {
        InvokePropertyChanged(nameof(CustomTitle));
        TitleChanged();
    }
    protected void TitleChanged()
    {
        SwitcherWindow.TitleText = Title;
        InvokePropertyChanged(nameof(Title));
    }
    public abstract IEnumerable<Window> Windows { get; }
    public abstract bool Selected { get; set; }
    public abstract bool IsDisposed { get; }

    public virtual void UpdateStatusLoop() { }

    public abstract void DetachAndDispose(bool JumpToCursor = false);
    public abstract Task TryCloseAsync();
    public abstract void Focus();
    public void TabCloseRequestedEv(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
    {
        ParentTabView.SelectedItem = sender;
        if (Settings.ExitOnClose)
            _ = TryCloseAsync();
        else
            DetachAndDispose(JumpToCursor: true);
    }
    public void TabClickEv(object sender, PointerRoutedEventArgs e) => Focus();
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void InvokePropertyChanged(string? PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    protected virtual void OnDoubleClick() { }
    public void TabDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        OnDoubleClick();
    }

    void SwitcherWindowFocusCallback()
    {

    }
    protected void OnIconChanged()
    {
        if (NativeIcon.HasValue)
            SwitcherWindow.SetLayeredWindowBitmap(NativeIcon.Value);
        SwitcherWindow.Bounds = SwitcherWindow.Bounds;
        InvokePropertyChanged(nameof(Icon));
    }
}
