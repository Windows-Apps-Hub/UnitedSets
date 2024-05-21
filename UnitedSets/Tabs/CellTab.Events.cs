using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Get.EasyCSharp;

namespace UnitedSets.Tabs;

partial class CellTab
{
    [Event(typeof(RoutedEventHandler))]
    public void ContentLoadEv(object sender)
    {
        // ContentPresentor does not update the selector when property changed is fired
        PropertyChanged += delegate
        {
            if (sender is ContentPresenter CP)
            {
                // Invalidate Content Template Selector
                var t = CP.ContentTemplateSelector;
                CP.ContentTemplateSelector = null;
                CP.ContentTemplateSelector = t;
                //CP.Visibility = Visibility;
            }
        };
    }
}
