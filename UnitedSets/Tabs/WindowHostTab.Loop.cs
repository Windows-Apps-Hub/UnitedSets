using System;
using Window = Microsoft.UI.Xaml.Window;
using CommunityToolkit.WinUI;

namespace UnitedSets.Tabs;

partial class WindowHostTab
{
    public override void UpdateStatusLoop()
    {
        if (_Title != DefaultTitle)
        {
            _Title = DefaultTitle;
            UIDispatcher?.TryEnqueue(() => InvokePropertyChanged(nameof(DefaultTitle)));
            if (!string.IsNullOrWhiteSpace(CustomTitle))
                UIDispatcher?.TryEnqueue(() => TitleChanged());
        }
        var icon = Window.LargeIcon;
        if (icon == default) icon = Window.SmallIcon;
        if (_Icon != icon)
        {
            _Icon = icon;
            UIDispatcher?.TryEnqueue(UpdateAppIcon);
        }
    }
}
