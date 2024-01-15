using System;

namespace WindowHoster;

public readonly record struct CompatablityMode(bool NoMoving, bool NoOwner)
{
    public bool IsActivateCropAllowed => !NoMoving;
    public static bool IsDwmBackdropSupported { get; } = Environment.OSVersion.Version.Build > 22621;

}
