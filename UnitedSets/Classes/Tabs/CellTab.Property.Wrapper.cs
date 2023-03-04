using Visibility = Microsoft.UI.Xaml.Visibility;

namespace UnitedSets.Classes.Tabs;

partial class CellTab
{
    Visibility Visibility => Selected ? Visibility.Visible : Visibility.Collapsed;
}
