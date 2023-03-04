using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using EasyCSharp;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    public bool IsSwitcherVisible { get; }

    public TabView ParentTabView { get; }

    public string Title => string.IsNullOrWhiteSpace(CustomTitle) ? DefaultTitle : CustomTitle;

    [Property(OnChanged = nameof(OnCustomTitleChanged))]
    string _CustomTitle = "";
    void OnCustomTitleChanged()
    {
        InvokePropertyChanged(nameof(CustomTitle));
        TitleChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
