using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
namespace UnitedSets.UI.Controls;
public partial class HomeBackground : StackPanel
{
	public HomeBackground()
	{
		InitializeComponent();
        var homePageSetting = UnitedSetsApp.Current.Settings.HomePageBackground;
        homePageSetting.PropertyChanged += delegate
        {
            hintInfo.Visibility = homePageSetting.Value ? Visibility.Visible : Visibility.Collapsed;
        };
        hintInfo.Visibility = homePageSetting.Value ? Visibility.Visible : Visibility.Collapsed;
    }
}
