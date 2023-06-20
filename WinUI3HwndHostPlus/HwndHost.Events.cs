using System;
using Microsoft.UI.Dispatching;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? Closed;
    public event Action? Updating;
}
