using System.Threading.Tasks;
using Get.XAMLTools;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using WinWrapper.Windowing;
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
        controller.RasterizationScale = XamlRoot.RasterizationScale;
        controller.ContainerRectangle = cachedContainerRectangle;
    }

    partial void OnAssociatedWindowChanged(RegisteredWindow? oldValue, RegisteredWindow? newValue)
    {
        Controller = null;
        if (oldValue is not null)
            oldValue.BecomesInvalid -= RemoveController;
        if (IsLoaded)
            AttachController(newValue);
    }

    async void AttachController(RegisteredWindow? window)
    {
        if (window is null) return;
        window.BecomesInvalid -= RemoveController;
        window.BecomesInvalid += RemoveController;
        // Another host may still own the window (e.g. while the previous tab content is being unloaded).
        // Retry for a short while instead of forever.
        for (int i = 0; i < 20; i++)
        {
            if (!IsLoaded || AssociatedWindow != window || !window.IsValid) return;
            try
            {
                Controller = window.GetController(ParentWindow, DispatcherQueue);
                return;
            }
            catch
            {
                await Task.Delay(100);
            }
        }
    }

    private void RemoveController()
    {
        if (DispatcherQueue.HasThreadAccess)
            Controller = null;
        else
            DispatcherQueue.TryEnqueue(() => Controller = null);
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
        if (XamlRoot is not null)
            controller.RasterizationScale = XamlRoot.RasterizationScale;
        controller.ContainerRectangle = cachedContainerRectangle;
    }
    XamlRoot? subscribedXamlRoot;
    private void WindowHost_Unloaded(object sender, RoutedEventArgs e)
    {
        Controller = null;
        if (subscribedXamlRoot is not null)
        {
            subscribedXamlRoot.Changed -= XamlRoot_Changed;
            subscribedXamlRoot = null;
        }
    }

    private void WindowHost_Loaded(object sender, RoutedEventArgs e)
    {
        Controller = null;
        Update(); // update position
        AttachController(AssociatedWindow);
        if (subscribedXamlRoot is not null)
            subscribedXamlRoot.Changed -= XamlRoot_Changed;
        subscribedXamlRoot = XamlRoot;
        wasVisible = XamlRoot.IsHostVisible;
        XamlRoot.Changed += XamlRoot_Changed;
    }
    bool wasVisible;
    private void XamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        // DPI change (e.g. moved to another monitor)
        Update();
        if (sender.IsHostVisible != wasVisible)
        {
            wasVisible = sender.IsHostVisible;
            if (AssociatedWindow is { } win && win.IsValid && Controller is not null)
            {
                var w = win.Window;
                w.IsVisible = sender.IsHostVisible;
            }
        }
    }
}
