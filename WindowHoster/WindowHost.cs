using Get.XAMLTools;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Window = WinWrapper.Windowing.Window;
namespace WindowHoster;
/// <summary>
/// The implementation of a WinUI <see cref="RegisteredWindow"/> host.
/// </summary>
[DependencyProperty<RegisteredWindow>("AssociatedWindow", UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
public partial class WindowHost : FrameworkElement
{
    Window ParentWindow =>
        Window.FromWindowHandle(
            (nint)XamlRoot.ContentIslandEnvironment.AppWindowId.Value
        );
    public WindowHost()
    {
        Loaded += WindowHost_Loaded;
        Unloaded += WindowHost_Unloaded;
        EffectiveViewportChanged += WindowHost_EffectiveViewportChanged;
    }
    Rect cachedContainerRectangle;
    private void WindowHost_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        Update();
    }
    void Update()
    {
        if (XamlRoot?.Content is not FrameworkElement rootElement) return;
        var margin = rootElement.Margin;
        var pos = TransformToVisual(rootElement).TransformPoint(
            new(margin.Left, margin.Top)
        );
        var size = ActualSize;
        cachedContainerRectangle = new()
        {
            X = pos.X,
            Y = pos.Y,
            Width = size.X,
            Height = size.Y
        };
        if (Controller is not { } controller) return;
        controller.ContainerRectangle = cachedContainerRectangle;
    }

    partial void OnAssociatedWindowChanged(RegisteredWindow? oldValue, RegisteredWindow? newValue)
    {
        Controller = null;
        if (oldValue is not null)
            oldValue.BecomesInvalid -= RemoveController;
        if (IsLoaded)
        {
            if (newValue is not null)
                newValue.BecomesInvalid += RemoveController;
            Controller = newValue?.GetController(ParentWindow, DispatcherQueue);
        }
    }

    private void RemoveController()
    {
        Controller = null;
    }

    RegisteredWindowController? Controller
    {
        get => _Controller;

        set
        {
            if (_Controller == value) return;
            BeforeControllerChanged();
            _Controller = value;
            AfterControllerChanged();
        }
    }
    RegisteredWindowController? _Controller;
    void BeforeControllerChanged()
    {
        Controller?.Unregister();
    }
    void AfterControllerChanged()
    {
        if (Controller is not { } controller) return;
        controller.ContainerRectangle = cachedContainerRectangle;
    }
    private void WindowHost_Unloaded(object sender, RoutedEventArgs e)
    {
        Controller = null;
    }

    private void WindowHost_Loaded(object sender, RoutedEventArgs e)
    {
        Controller = null;
        Update(); // update position
        if (AssociatedWindow is { } window)
        {
            window.BecomesInvalid += RemoveController;
            Controller = window.GetController(ParentWindow, DispatcherQueue);
        }
        XamlRoot.Changed += XamlRoot_Changed;
    }
    bool wasVisible;
    private void XamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        if (XamlRoot.IsHostVisible != wasVisible)
        {
            wasVisible = XamlRoot.IsHostVisible;
            if (AssociatedWindow is { } win && win.IsValid && Controller is not null)
            {
                var w = win.Window;
                w.IsVisible = XamlRoot.IsHostVisible;
            }
        }
    }
}
