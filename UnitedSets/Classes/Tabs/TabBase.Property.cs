using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using EasyCSharp;
using Microsoft.UI.Xaml.Media;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    public bool IsSwitcherVisible { get; }

    public string Title => string.IsNullOrWhiteSpace(CustomTitle) ? DefaultTitle : CustomTitle;

    [Property(OnChanged = nameof(OnCustomTitleChanged))]
    string _CustomTitle = "";
    void OnCustomTitleChanged()
    {
        InvokePropertyChanged(nameof(CustomTitle));
        TitleChanged();
    }
	[AutoNotifyProperty]
	Brush? _HeaderBackgroundBrush; //not yet used
	[AutoNotifyProperty]
	Brush? _HeaderForegroundBrush; //not yet used

    public event PropertyChangedEventHandler? PropertyChanged;
}
