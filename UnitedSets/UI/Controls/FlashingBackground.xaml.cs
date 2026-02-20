namespace UnitedSets.UI.Controls;

public sealed partial class WindowHoverIndicatorBackground : Grid
{
    public WindowHoverIndicatorBackground() => InitializeComponent();
    public void TransitionToHoverState()
        => DispatcherQueue.TryEnqueue(() => WindowHoveringStoryBoard.Begin());
    public void TransitionToNotHoverState()
        => DispatcherQueue.TryEnqueue(() => NoWindowHoveringStoryBoard.Begin());
}
