using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using Get.EasyCSharp;
using Microsoft.UI.Xaml.Media;

namespace UnitedSets.Tabs;

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
    [AutoNotifyProperty(OnChanged = nameof(FlashingChanged))]
    bool _IsFlashing;
    void FlashingChanged()
    {
        if (UnitedSetsApp.Current.SelectedTab == this)
        {
            // no, do not allow flashing on this tab.
            IsFlashing = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
