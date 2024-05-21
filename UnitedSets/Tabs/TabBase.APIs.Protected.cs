using System.ComponentModel;

namespace UnitedSets.Tabs;

partial class TabBase
{
    protected void TitleChanged()
    {
        ChangeSwitcherTitle();
        InvokePropertyChanged(nameof(Title));
    }
    
    protected void OnIconChanged()
    {
        ChangeSwitcherIcon();
        InvokePropertyChanged(nameof(Icon));
    }

    protected void InvokePropertyChanged(string? PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
}
