using UnitedSets.UI.Controls;
using UnitedSets.UI.FlyoutModules;

namespace UnitedSets.Tabs;

public abstract partial class TabBase : INotifyPropertyChanged
{
	public const bool DefaultIsSwitcherVisible = true;
	[AutoNotifyProperty]
	bool _IsSelected;
    [OptionalParameter(nameof(IsSwitcherVisible), DefaultIsSwitcherVisible)]
    public TabBase(bool IsSwitcherVisible)
    {
		AllTabs.Add(this);
        this.IsSwitcherVisible = IsSwitcherVisible;
        InitSwitcher();
    }
    /// <summary>
    /// The set window this tab currently belongs to (maintained by <see cref="MainWindow.Tabs"/>).
    /// </summary>
    public MainWindow? ParentWindow { get; internal set; }

    /// <summary>
    /// Whether the given top-level window is hosted by this tab.
    /// </summary>
    public virtual bool ContainsWindow(nint hwnd) => Windows.Any(x => x.Handle == hwnd);
    public abstract bool ContainsRegisteredWindow(WindowHoster.RegisteredWindow window);

	protected virtual void DoShowTab() {
		ParentWindow?.SelectedTab = this;
    }
    protected void ShowFlyout(UIElement Element, UIElement RelativeTo)
    {
        ShowFlyout([Element], RelativeTo);
    }

    protected async void ShowFlyout(UIElement[] Elements, UIElement RelativeTo) {
		await Task.Delay(300);
        StackPanel sp;
        var flyout = new BackdropedFlyout
        {
            Content = sp = new StackPanel
            {
                Width = 350,
                Spacing = 8,
                Children =
                {
                    new BasicTabFlyoutModule(this)
                }
            },
            ShouldConstrainToRootBounds = false,
            Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom
        };
        foreach (var ele in Elements)
            sp.Children.Add(ele);
        flyout.ShowAt((FrameworkElement)RelativeTo);
    }
	protected virtual void DoRemoveTab()
    {
        UnitedSetsApp.Current.DispatcherQueue.TryEnqueue(() =>
        {
            if (ParentWindow is { } window)
            {
                window.Tabs.Remove(this);
                window.CloseIfEmptyAfterTabMovedOut();
            }
            // the tab may also be set aside
            foreach (var group in UnitedSetsApp.Current.HiddenTabs.ToArray())
                group.Tabs.Remove(this);
        });
    }
}
