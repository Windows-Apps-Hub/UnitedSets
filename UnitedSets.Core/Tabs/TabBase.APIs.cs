using EasyCSharp;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    // Abstract
    public abstract void DetachAndDispose(bool JumpToCursor = false);
    public abstract Task TryCloseAsync();

    [Event(typeof(PointerEventHandler), Name = "TabClickEv")]
    public abstract void Focus();
    // Virtual
    public virtual partial void UpdateStatusLoop();
    [Event(typeof(DoubleTappedEventHandler), Name = "TabDoubleTapped", Visibility = GeneratorVisibility.Public)]
    protected virtual partial void OnDoubleClick([Cast] UIElement sender, DoubleTappedRoutedEventArgs args);
    // Public
    [RelayCommand]
    public partial void TryCloseNoWait();
    // Protected
    protected partial void TitleChanged();
    protected partial void OnIconChanged();
    protected partial void InvokePropertyChanged(string? PropertyName);
    // Private
    [RelayCommand]
    private partial void DetachWindow();
}
