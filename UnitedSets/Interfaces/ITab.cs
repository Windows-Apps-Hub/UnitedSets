using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedSets.Classes;

namespace UnitedSets.Interfaces
{
    interface ITab
    {
        BitmapImage? Icon { get; }
        string Title { get; }
        HwndHost HwndHost { get; }
        bool Selected { get; set; }
    }
}
