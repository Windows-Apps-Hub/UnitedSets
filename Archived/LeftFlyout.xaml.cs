using EasyCSharp;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using UnitedSets.Helpers;
using WindowEx = WinWrapper.Window;
using static WinUIEx.WindowExtensions;
using Windows.Foundation;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets.Windows.Flyout;

public sealed partial class LeftFlyout : INotifyPropertyChanged
{
    readonly bool SignletonMode;
    readonly WindowEx CurrentWindowEx;
    public LeftFlyout(WindowEx ParentWindow, params UIElement[] Modules) : this(false, ParentWindow, Modules) { }
    public static LeftFlyout CreateSingletonMode(WindowEx ParentWindow, params UIElement[] Modules)
        => new(true, ParentWindow, Modules);
    IWindowFlyoutModule[] RegisteredEventModules;
    [AutoNotifyProperty]
    string _HeaderText = "Settings";
    WindowEx ParentWindow;
    private LeftFlyout(bool SignletonMode, WindowEx ParentWindow, params UIElement[] Modules)
    {
        this.ParentWindow = ParentWindow;
        this.SignletonMode = SignletonMode;
        InitializeComponent();
        foreach (var Module in Modules)
            ModuleContainer.Children.Add(Module);
        
        RegisteredEventModules = Modules.Where(x => x is IWindowFlyoutModule).Select(x => (IWindowFlyoutModule)x).ToArray();
        
        foreach (var modules in RegisteredEventModules)
            modules.RequestClose += CloseClick;

        CurrentWindowEx = WindowEx.FromWindowHandle(
            WinRT.Interop.WindowNative.GetWindowHandle(this)
        );
        this.SetForegroundWindow();
        var parentbounds = ParentWindow.Bounds;
        CurrentWindowEx.Bounds = CurrentWindowEx.Bounds with
        {
            X = Math.Max(10, parentbounds.X - 455),
            Y = parentbounds.Y
        };
        Activated += OnActivatedChanged;
        AppWindow.Closing += AppWindow_Closing;
    }

    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate)]
    bool _IsDisposed;
    private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        _IsDisposed = true;
    }

    TaskCompletionSource? ShowTaskCompletion;
	public static bool NoAutoClose;

    [Event(typeof(TypedEventHandler<object, WindowActivatedEventArgs>))]
    void OnActivatedChanged(WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
			if (NoAutoClose)
				return;

			ShowTaskCompletion?.TrySetResult();
            ShowTaskCompletion = null;
        } else
        {
            foreach (var modules in RegisteredEventModules)
            {
                modules.OnActivated();
            }
        }
    }

    public async Task ShowAsync()
    {
        var parentbounds = ParentWindow.Bounds;
        var CurrentWindowEx = this.CurrentWindowEx;
        CurrentWindowEx.Bounds = CurrentWindowEx.Bounds with
        {
            X = Math.Max(10, parentbounds.X - 455),
            Y = parentbounds.Y
        };
        AppWindow.Show();
        await Task.Delay(100);
        Activate();
        ShowTaskCompletion ??= new();
        await ShowTaskCompletion.Task;
    }

    [Event(typeof(RoutedEventHandler))]
    void CloseClick() {
        if (SignletonMode) AppWindow.Hide(); else Close();
    }
    [AutoNotifyProperty(OnChanged = nameof(ExtendToTopChanged))]
    bool _ExtendToTop = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    void ExtendToTopChanged()
    {
        if (_ExtendToTop)
        {
            Grid.SetRow(ScrollViewer, 0);
            Grid.SetRowSpan(ScrollViewer, 2);
        } else
        {
            Grid.SetRow(ScrollViewer, 1);
            Grid.SetRowSpan(ScrollViewer, 1);
        }
    }
}
