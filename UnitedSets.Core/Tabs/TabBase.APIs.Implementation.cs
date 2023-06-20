using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    public virtual partial void UpdateStatusLoop() { }
    protected virtual partial void OnDoubleClick(UIElement sender, DoubleTappedRoutedEventArgs args) { }
    public async partial void TryCloseNoWait() => await TryCloseAsync();
    private partial void DetachWindow() => DetachAndDispose(JumpToCursor: false);
    protected partial void TitleChanged()
    {
        ChangeSwitcherTitle();
        InvokePropertyChanged(nameof(Title));
    }
    
    protected partial void OnIconChanged()
    {
        ChangeSwitcherIcon();
        InvokePropertyChanged(nameof(Icon));
    }

    protected partial void InvokePropertyChanged(string? PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
}
