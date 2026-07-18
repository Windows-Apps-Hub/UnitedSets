using UnitedSets.Cells.Data;
using UnitedSets.Tabs;
using UnitedSets.UI.Controls.Cells;
using WindowHoster;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;

namespace UnitedSets.UI.Controls;
[DependencyProperty<TabBase>("Tab", UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
public partial class TabVisualizer : TemplateControl<Grid>
{
    Grid? rootElement;
    readonly Dictionary<TabBase, UIElement> _cachedVisuals = [];
    nint _thumbnail;
    nint _thumbnailSource;

    protected override void Initialize(Grid rootElement)
    {
        this.rootElement = rootElement;
        SizeChanged += (_, _) => UpdateThumbnailBounds();
        Unloaded += (_, _) => ClearThumbnail();
        OnTabChanged(null, Tab);
    }

    UIElement GetOrCreateVisual(TabBase tab)
    {
        if (_cachedVisuals.TryGetValue(tab, out var cached))
            return cached;

        UIElement visual = tab switch
        {
            CellTab ct => (UIElement)new GenericCellVisualizer(ct.MainCell).WithCustomCode(vis =>
            {
                ct.MainCellProperty.ApplyAndRegisterForNewValue(x => vis.Cell = x);
                ct.CellMarginProperty.ApplyAndRegisterForNewValue(x => vis.CellMargin = x);
            }),
            WindowHostTab wt => (UIElement)new WindowHost { AssociatedWindow = wt.RegisteredWindow },
            _ => throw new System.InvalidCastException("Unknown tab type")
        };

        _cachedVisuals[tab] = visual;
        return visual;
    }

    void SetAllVisualsHiddenExcept(UIElement? toShow)
    {
        if (rootElement is null) return;
        foreach (var child in rootElement.Children)
        {
            var isSelected = child == toShow;
            child.Opacity = isSelected ? 1 : 0;
            child.IsHitTestVisible = isSelected;
        }
    }

    static void ActivateWindows(TabBase tab)
    {
        foreach (var window in tab.Windows)
        {
            if (window.IsValid)
            {
                if (IsIconic(window.Handle))
                    _ = ShowWindowAsync(window.Handle, 9);
                _ = SetWindowPos(window.Handle, 0, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x4000);
                _ = SetForegroundWindow(window.Handle);
            }
        }
    }

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsIconic(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindowAsync(nint window, int command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(nint window);

    void UpdateThumbnail(TabBase tab)
    {
        if (tab is WindowHostTab windowTab && windowTab.RegisteredWindow.UsesOwnerHosting)
        {
            ClearThumbnail();
            return;
        }

        var source = tab.Windows.FirstOrDefault(x => x.IsValid).Handle;
        if (source == 0 || XamlRoot is null) return;

        if (_thumbnailSource == source)
        {
            UpdateThumbnailBounds();
            return;
        }

        var destination = (nint)XamlRoot.ContentIslandEnvironment.AppWindowId.Value;
        if (DwmRegisterThumbnail(destination, source, out var thumbnail) != 0)
            return;

        var oldThumbnail = _thumbnail;
        _thumbnail = thumbnail;
        _thumbnailSource = source;
        UpdateThumbnailBounds();
        if (oldThumbnail != 0)
            _ = DwmUnregisterThumbnail(oldThumbnail);
    }

    void UpdateThumbnailBounds()
    {
        if (_thumbnail == 0 || XamlRoot?.Content is not FrameworkElement root) return;

        var position = TransformToVisual(root).TransformPoint(default);
        var scale = XamlRoot.RasterizationScale;
        var properties = new DwmThumbnailProperties
        {
            Flags = 0x00000001 | 0x00000008 | 0x00000010,
            Destination = new NativeRect
            {
                Left = (int)Math.Round(position.X * scale),
                Top = (int)Math.Round(position.Y * scale),
                Right = (int)Math.Round((position.X + ActualWidth) * scale),
                Bottom = (int)Math.Round((position.Y + ActualHeight) * scale)
            },
            Visible = 1,
            SourceClientAreaOnly = 0
        };
        _ = DwmUpdateThumbnailProperties(_thumbnail, ref properties);
    }

    void ClearThumbnail()
    {
        if (_thumbnail != 0)
            _ = DwmUnregisterThumbnail(_thumbnail);
        _thumbnail = 0;
        _thumbnailSource = 0;
    }

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmRegisterThumbnail(nint destination, nint source, out nint thumbnail);

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmUnregisterThumbnail(nint thumbnail);

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmUpdateThumbnailProperties(nint thumbnail, ref DwmThumbnailProperties properties);

    [StructLayout(LayoutKind.Sequential)]
    struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DwmThumbnailProperties
    {
        public uint Flags;
        public NativeRect Destination;
        public NativeRect Source;
        public byte Opacity;
        public int Visible;
        public int SourceClientAreaOnly;
    }

    partial void OnTabChanged(TabBase? oldValue, TabBase? newValue)
    {
        if (rootElement is null) return;

        if (newValue is null)
        {
            ClearThumbnail();
            SetAllVisualsHiddenExcept(null);
            return;
        }

        var visual = GetOrCreateVisual(newValue);
        if (!rootElement.Children.Contains(visual))
            rootElement.Children.Add(visual);

        SetAllVisualsHiddenExcept(visual);
        UpdateThumbnail(newValue);
        ActivateWindows(newValue);
    }
}
