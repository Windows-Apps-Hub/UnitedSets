namespace UnitedSets.UI.Controls;

public sealed partial class WindowHoverIndicatorBackground : Grid
{
    public WindowHoverIndicatorBackground() => InitializeComponent();
#pragma warning disable IDE0200 // Remove unnecessary lambda expression
    public void TransitionToHoverState()
        => DispatcherQueue.TryEnqueue(() => WindowHoveringStoryBoard.Begin());

    public void TransitionToNotHoverState()
        => DispatcherQueue.TryEnqueue(() => NoWindowHoveringStoryBoard.Begin());
#pragma warning restore IDE0200 // Remove unnecessary lambda expression
}
