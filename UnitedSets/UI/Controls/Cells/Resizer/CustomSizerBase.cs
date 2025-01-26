// Most logics are from Community Toolkit
// Original: https://github.com/CommunityToolkit/Windows/blob/main/components/Sizers/src/
// This is a (simplified) port to XACL.

using System;
using Get.Data.Properties;
using Get.Data.Bindings.Linq;
using Get.UI.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using Get.Data.Bindings;
namespace UnitedSets.UI.Controls.Cells.Resizer;
[AutoProperty]
public abstract partial class CustomSizerBase : TemplateControl<Grid>
{
    public CustomSizerBase()
    {
        // default styles
        IsTabStop = true;
        UseSystemFocusVisuals = true;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        IsFocusEngagementEnabled = true;
        MinWidth = 8;
        MinHeight = 8;
        Padding = new(4); // SizerBasePadding
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;
        ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
    }
    public IProperty<Orientation> OrientationProperty { get; } = Auto(Orientation.Vertical);
    public IProperty<Visibility> ThumbVisibilityProperty { get; } = Auto(Visibility.Visible);
    public IProperty<double> DragIncrementProperty { get; } = Auto(1d);
    public IProperty<double> KeyboardIncrementProperty { get; } = Auto(8d);
    [Flags]
    enum SizerBasePointerStates
    {
        None = 0,
        Pressed = 0b01,
        Over = 0b10
    }
    readonly IProperty<SizerBasePointerStates> PointerStates = Auto(SizerBasePointerStates.None);
    protected override void Initialize(Grid RootGrid)
    {

        ThemeResources.Get<Brush>("ControlStrongFillColorDefaultBrush", this).ApplyAndRegisterForNewValue((_, x) =>
        {
            Foreground = x;
        });
        ThemeResources.Get<Brush>("ControlAltFillColorTransparentBrush", this).ApplyAndRegisterForNewValue((_, x) =>
        {
            Background = x;
        });

        RootGrid.SetValueBindOneWay(Grid.BackgroundProperty, (this, BackgroundProperty));
        RootGrid.SetValueBindOneWay(Grid.BorderThicknessProperty, (this, BorderThicknessProperty));
        RootGrid.SetValueBindOneWay(Grid.CornerRadiusProperty, (this, CornerRadiusProperty));
        RootGrid.BackgroundTransition = new BrushTransition { Duration = TimeSpan.FromMilliseconds(83) };
        RootGrid.BackgroundTransition = new BrushTransition { Duration = TimeSpan.FromMilliseconds(83) };
        var SizerBaseBackgroundPointerOver = ThemeResources.Get<Brush>("ControlAltFillColorTertiaryBrush", this);
        var SizerBaseBackgroundPressed = ThemeResources.Get<Brush>("ControlAltFillColorQuarternaryBrush", this);
        var SizerBaseBackgroundDisabled = ThemeResources.Get<Brush>("ControlAltFillColorDisabledBrush", this);
        var output =
            from ptstate in PointerStates
            from bgptover in SizerBaseBackgroundPointerOver
            from bgpressed in SizerBaseBackgroundPressed
            from bgdisabled in SizerBaseBackgroundDisabled
            select ptstate.HasFlag(SizerBasePointerStates.Pressed)
            ? bgpressed : (
                ptstate.HasFlag(SizerBasePointerStates.Over) ?
                bgptover :
                Solid(Colors.Transparent)
            // disabled not handled
            );
        output.ApplyAndRegisterForNewValue((_, x) => RootGrid.Background = x);
        Rectangle PART_Thumb;
        RootGrid.Children.Add(PART_Thumb = new Rectangle());
        var SizerBaseThumbWidth = Auto(4d); //ThemeResources.Get<double>("SizerBaseThumbWidth", this);
        var SizerBaseThumbHeight = Auto(24d); //ThemeResources.Get<double>("SizerBaseThumbHeight", this);
        var SizerBaseThumbRadius = Auto(2d); // ThemeResources.Get<double>("SizerBaseThumbRadius", this);
        SizerBaseThumbWidth.ApplyAndRegisterForNewValue((_, x) => PART_Thumb.Width = x);
        SizerBaseThumbHeight.ApplyAndRegisterForNewValue((_, x) => PART_Thumb.Height = x);
        SizerBaseThumbRadius.ApplyAndRegisterForNewValue((_, x) =>
        {
            PART_Thumb.RadiusX = x;
            PART_Thumb.RadiusY = x;
        });
        RootGrid.SetValueBindOneWay(Rectangle.MarginProperty, (this, PaddingProperty));
        RootGrid.SetValueBindOneWay(Rectangle.FillProperty, (this, ForegroundProperty));
        ThumbVisibilityProperty.ApplyAndRegisterForNewValue((_, x) => PART_Thumb.Visibility = x);
        var thumbSize = from orientation in OrientationProperty
                        from w in SizerBaseThumbWidth
                        from h in SizerBaseThumbHeight
                        select orientation is Orientation.Vertical ? (w, h) : (h, w);
        thumbSize.ApplyAndRegisterForNewValue((_, x) =>
        {
            PART_Thumb.Width = x.Item1;
            PART_Thumb.Height = x.Item2;
        });
        OrientationProperty.ApplyAndRegisterForNewValue((_, x) =>
        {
            if (x is Orientation.Vertical)
            {
                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
            }
            else
            {
                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
            }
        });
        RootGrid.PointerEntered += delegate
        {
            PointerStates.CurrentValue |= SizerBasePointerStates.Over;
        };
        RootGrid.PointerPressed += delegate
        {
            PointerStates.CurrentValue |= SizerBasePointerStates.Pressed;
        };
        RootGrid.PointerReleased += delegate
        {
            PointerStates.CurrentValue &= ~SizerBasePointerStates.Pressed;
        };
        RootGrid.PointerExited += delegate
        {
            PointerStates.CurrentValue &= ~SizerBasePointerStates.Over;
        };
    }
    protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
    {
        base.OnManipulationStarting(e);

        PointerStates.CurrentValue |= SizerBasePointerStates.Pressed;
        OnDragStarting();
    }

    protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
    {
        var horizontalChange =
            Math.Truncate(e.Cumulative.Translation.X / DragIncrement) * DragIncrement;
        var verticalChange =
            Math.Truncate(e.Cumulative.Translation.Y / DragIncrement) * DragIncrement;

        if (FlowDirection is FlowDirection.RightToLeft)
            horizontalChange *= -1;

        if (Orientation is Orientation.Vertical)
        {
            if (!OnDragHorizontal(horizontalChange))
                return;
        }
        else
        {
            if (!OnDragVertical(verticalChange))
                return;
        }

        base.OnManipulationDelta(e);
    }
    protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
    {
        base.OnManipulationCompleted(e);

        PointerStates.CurrentValue &= ~SizerBasePointerStates.Pressed;
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        if (PointerStates.CurrentValue.HasFlag(SizerBasePointerStates.Pressed)) return;

        // Initialize a drag event for this keyboard interaction.
        OnDragStarting();

        if (Orientation is Orientation.Vertical)
        {
            var horizontalChange = KeyboardIncrement;

            if (FlowDirection == FlowDirection.RightToLeft)
                horizontalChange *= -1;

            if (e.Key is Windows.System.VirtualKey.Left)
                OnDragHorizontal(-horizontalChange);
            else if (e.Key is Windows.System.VirtualKey.Right)
                OnDragHorizontal(horizontalChange);
        }
        else
        {
            if (e.Key is Windows.System.VirtualKey.Up)
                OnDragVertical(-KeyboardIncrement);
            else if (e.Key is Windows.System.VirtualKey.Down)
                OnDragVertical(KeyboardIncrement);
        }
    }
    protected abstract void OnDragStarting();
    protected abstract bool OnDragHorizontal(double horizontalChange);
    protected abstract bool OnDragVertical(double verticalChange);

}

static class BindingExtension
{
    public static void SetValueBindOneWay(this (DependencyObject obj, DependencyProperty prop) dest, (DependencyObject obj, DependencyProperty prop) src)
    {
        src.obj.RegisterPropertyChangedCallback(src.prop, delegate
        {
            dest.obj.SetValue(dest.prop, src.obj.GetValue(src.prop));
        });
    }
    public static void SetValueBindOneWay(this DependencyObject dest, DependencyProperty prop, (DependencyObject obj, DependencyProperty prop) src)
    {
        (dest, prop).SetValueBindOneWay(src);
    }
}
