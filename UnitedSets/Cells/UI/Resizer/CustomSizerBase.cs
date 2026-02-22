namespace UnitedSets.UI.Controls.Cells.Resizer;

// Most logics are from Community Toolkit
// Original: https://github.com/CommunityToolkit/Windows/blob/main/components/Sizers/src/
// This is a (simplified) port to QuickMarkup.

[QuickMarkup("""
    using Rectangle = Microsoft.UI.Xaml.Shapes.Rectangle;
    Orientation Orientation = Vertical;
    Visibility ThumbVisibility = Visible;
    double DragIncrement = 1;
    double KeyboardIncrement = 8;

    private SizerBasePointerStates PointerStates = None;
    <setup>
    var SizerBaseBackgroundPointerOver = ThemeResources.Get<Brush>("ControlAltFillColorTertiaryBrush", this).CreateReadOnlyReference();
    var SizerBaseBackgroundPressed = ThemeResources.Get<Brush>("ControlAltFillColorQuarternaryBrush", this).CreateReadOnlyReference();
    // var SizerBaseBackgroundDisabled = ThemeResources.Get<Brush>("ControlAltFillColorDisabledBrush", this).CreateReadOnlyReference();
    var SizerBaseThumbWidth = Ref(4d); //ThemeResources.Get<double>("SizerBaseThumbWidth", this);
    var SizerBaseThumbHeight = Ref(24d); //ThemeResources.Get<double>("SizerBaseThumbHeight", this);
    var SizerBaseThumbRadius = Ref(2d); // ThemeResources.Get<double>("SizerBaseThumbRadius", this);

    var we = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
    var ns = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
    </setup>
    <root
        // default styles
        IsTabStop UseSystemFocusVisuals IsFocusEngagementEnabled
        StretchH StretchV
        MinWidth=8 MinHeight=8
        Padding=4 // SizerBasePadding
        HorizontalContentAlignment=Center
        VerticalContentAlignment=Center
        ManipulationMode=`ManipulationModes.TranslateX | ManipulationModes.TranslateY`
        ProtectedCursor=`Orientation is Orientation.Vertical ? we : ns`
    >
        RootGrid = <Grid
            BackgroundTransition=<BrushTransition Duration=`TimeSpan.FromMilliseconds(83)` />
            Background=`
                PointerStates.HasFlag(SizerBasePointerStates.Pressed) ? SizerBaseBackgroundPressed.Value : (
                PointerStates.HasFlag(SizerBasePointerStates.Over) ? SizerBaseBackgroundPointerOver.Value : (
                    // disabled case not handled
                    // on case of not over
                    Solid(Colors.Transparent)
                ))            
            `
            @PointerEntered+=`PointerStates |= SizerBasePointerStates.Over`
            @PointerPressed+=`PointerStates |= SizerBasePointerStates.Pressed`
            @PointerReleased+=`PointerStates &= ~SizerBasePointerStates.Pressed`
            @PointerExited+=`PointerStates &= ~SizerBasePointerStates.Over`
        >
            PART_Thumb = <Rectangle
                Width=`Orientation is Orientation.Vertical ? SizerBaseThumbWidth.Value : SizerBaseThumbHeight.Value`
                Height=`Orientation is Orientation.Vertical ? SizerBaseThumbHeight.Value : SizerBaseThumbWidth.Value`
                RadiusX=`SizerBaseThumbRadius.Value`
                RadiusY=`SizerBaseThumbRadius.Value`
                Visibility=`ThumbVisibility`
            />
        </Grid>
    </root>
    """)]
public abstract partial class CustomSizerBase : UserControl
{
    public CustomSizerBase()
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
        PART_Thumb.SetValueBindOneWay(Rectangle.MarginProperty, (this, PaddingProperty));
        PART_Thumb.SetValueBindOneWay(Rectangle.FillProperty, (this, ForegroundProperty));
        Init();
    }
    [Flags]
    enum SizerBasePointerStates
    {
        None = 0,
        Pressed = 0b01,
        Over = 0b10
    }
    protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
    {
        base.OnManipulationStarting(e);

        PointerStates |= SizerBasePointerStates.Pressed;
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

        PointerStates &= ~SizerBasePointerStates.Pressed;
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        if (PointerStates.HasFlag(SizerBasePointerStates.Pressed)) return;

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
        var (srcObj, srcProp) = src;
        var (destObj, destProp) = src;
        srcObj.RegisterPropertyChangedCallback(srcProp, delegate
        {
            destObj.SetValue(destProp, srcObj.GetValue(srcProp));
        });
    }
    public static void SetValueBindOneWay(this DependencyObject dest, DependencyProperty prop, (DependencyObject obj, DependencyProperty prop) src)
    {
        (dest, prop).SetValueBindOneWay(src);
    }
}
