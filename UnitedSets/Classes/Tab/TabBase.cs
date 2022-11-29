using EasyCSharp;
using Microsoft.Extensions.DependencyInjection;
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
using WinWrapper;

namespace UnitedSets.Classes.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
    public readonly static List<Window> MainWindows = new();
    public static event Action? OnUpdateStatusLoopComplete;
    static readonly SynchronizedCollection<TabBase> AllTabs = new();
    static TabBase()
    {
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

    public TabBase(TabView Parent)
    {
        AllTabs.Add(this);
        ParentTabView = Parent;
    }

    public TabView ParentTabView { get; }
    public abstract BitmapImage? Icon { get; }
    public abstract string DefaultTitle { get; }

    public string Title => string.IsNullOrWhiteSpace(CustomTitle) ? DefaultTitle : CustomTitle;
    
    [Property(OnChanged = nameof(OnCustomTitleChanged))]
    string _CustomTitle = "";
    void OnCustomTitleChanged()
    {
        InvokePropertyChanged(nameof(CustomTitle));
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
}
