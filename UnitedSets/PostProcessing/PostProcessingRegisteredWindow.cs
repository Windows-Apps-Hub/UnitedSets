using System.Threading.Tasks;
using WindowHoster;
using WinWrapper.Input;
using WinWrapper.Windowing;

namespace UnitedSets.PostProcessing;
public static class PostProcessingRegisteredWindow
{
    public static RegisteredWindow? Register(Window window, bool shouldBeHidden = false)
    {
        var r = RegisteredWindow.Register(window, shouldBeHidden);
        if (r is null) return null;
        var fn = Utils.GetOwnerProcessModuleFilename(window);
        if (fn is not null && UnitedSetsApp.Current.Configuration.MainConfiguration.
            DefaultWindowStylesData.TryGetValue(fn, out var saved))
        {
            if ((saved.Borderless ?? false) || Keyboard.IsAltDown)
                r.Properties.BorderlessWindow = true;
            if (saved.CropEnabled ?? false)
                r.Properties.ActivateCrop = true;
            if (saved.CropRect is { } cr)
                r.Properties.CropRegion = cr;
        }
        else
        {
            r.Properties.BorderlessWindow = Keyboard.IsAltDown;
        }
        return r;
    }
}
