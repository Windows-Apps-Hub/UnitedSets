using Microsoft.UI.Xaml.Media.Imaging;
using UnitedSets.Classes;

namespace UnitedSets.Interfaces;

public interface ITab
{
    BitmapImage? Icon { get; }
    string Title { get; }
    HwndHost HwndHost { get; }
    bool Selected { get; set; }
}
