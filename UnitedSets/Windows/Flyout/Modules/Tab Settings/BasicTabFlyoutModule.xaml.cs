using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes.Tabs;
using EasyCSharp;
using WinUI3HwndHostPlus;
using System;

namespace UnitedSets.Windows.Flyout.Modules;

public sealed partial class BasicTabFlyoutModule : IWindowFlyoutModule {
    public BasicTabFlyoutModule(TabBase TabBase)
    {
        this.TabBase = TabBase;
        InitializeComponent();
    }
    readonly TabBase TabBase;

	public event Action RequestClose;

	[Event(typeof(RoutedEventHandler))]
	void DetachWindow() {


		TabBase.DetachAndDispose();
		RequestClose?.Invoke();
	}

	[Event(typeof(TextChangedEventHandler))]
    private void TabNameTextBoxChanged()
    {
        TabBase.CustomTitle = TabNameTextBox.Text;
    }

    [Event(typeof(RoutedEventHandler))]
    private void TabNameReset()
    {
        TabNameTextBox.Text = "";
    }

	public void OnActivated() {
		
	}
}
