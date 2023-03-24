using System;
using Window = Microsoft.UI.Xaml.Window;
using CommunityToolkit.WinUI;

namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    public override void UpdateStatusLoop()
    {
        if (_Title != DefaultTitle)
        {
            _Title = DefaultTitle;
            UIDispatcher?.EnqueueAsync(() => InvokePropertyChanged(nameof(DefaultTitle)));
            if (!string.IsNullOrWhiteSpace(CustomTitle))
                UIDispatcher?.EnqueueAsync(() => TitleChanged());
        }
        var icon = Window.LargeIconPtr;
        if (icon == IntPtr.Zero) icon = Window.SmallIconPtr;
        if (_Icon != icon)
        {
            _Icon = icon;
            UIDispatcher?.EnqueueAsync(UpdateAppIcon);
        }
    }
}
