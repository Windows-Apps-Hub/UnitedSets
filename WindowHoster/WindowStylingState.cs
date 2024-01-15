using System.Drawing;
using WinWrapper.Windowing;
using WinWrapper;
using System;

namespace WindowHoster;

readonly record struct WindowStylingState(
    WindowStyles Style,
    WindowExStyles ExStyles,
    DwmSystemBackdropType Backdrop,
    Rectangle? Region,
    bool IsResizable
)
{
    public static WindowStylingState GetCurrentState(Window window)
        => new(
            window.Style,
            window.ExStyle,
            CompatablityMode.IsDwmBackdropSupported ?
                window.DwmAttribute.Get<DwmSystemBackdropType>(DwmWindowAttribute.SystemBackdropTypes) :
                DwmSystemBackdropType.None,
            window.Region,
            window.IsResizable
        );
    public void RestoreTo(Window window)
    {
        window.Style = Style;
        window.ExStyle = ExStyles;
        if (CompatablityMode.IsDwmBackdropSupported)
            window.DwmAttribute.Set(DwmWindowAttribute.SystemBackdropTypes, Backdrop);
        window.Region = Region;
        window.IsResizable = IsResizable;
    }
}

public enum DwmSystemBackdropType : uint
{
    Auto,
    None,
    MainWindow,
    TransientWindow,
    TabbedWindow
}