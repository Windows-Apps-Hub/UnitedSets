using Microsoft.UI.Xaml.Controls;
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
        IconSource? Icon { get; }
        BitmapImage? Tempicon { get; }
        string Title { get; }
        HwndHost HwndHost { get; }
        bool Selected { get; set; }
    }
}
