using System.Drawing;
using WindowEx = WinWrapper.Windowing.Window;
using WinWrapper.Windowing;
using System.Linq;
using System.Threading.Tasks;

namespace WinUI3HwndHostPlus;

partial class HwndHost
{
    int CountDown = 5;
	public async void ClearCrop() {
		CropLeft = CropRight = CropBottom = CropTop = 0;
		await WindowInfo.HostedWindow.SetRegionAsync(null);//could do initial region as well
	}
	async void OnWindowUpdate()
    {
        if (_CacheWindowRect is { Width: 0 } or { Height: 0 }) return; // wait for update
        if (IsDisposed) return;

        var HostedWindow = WindowInfo.HostedWindow;
        var ParentWindow = WindowInfo.XAMLWin32Window;
        if (!IsWindowVisible)
        {
            HostedWindow.IsVisible = false;
            return;
        }

        bool Check = false;
        if (CountDown > 0)
        {
            CountDown--;
            if (CountDown == 0) HostedWindow.Redraw();
        }
        else Check = true;
        var windowbounds = ParentWindow.Bounds;

        var scale = GetScale(ParentWindow);
        var Pt = new Point
        {
            X = (int)(windowbounds.X + _CacheWindowRect.X),
            Y = (int)(windowbounds.Y + _CacheWindowRect.Y)
        };


        try
        {
			if (HostedWindow.IsResizable)
				HostedWindow.IsResizable = false;
        }
        catch
        {

        }
        if (!HostedWindow.IsValid)
        {
            Dispose();
            return;
        }
        Updating?.Invoke();
        var YShift = ParentWindow.IsMaximized ? 8 : 0;
        if (!CompatabilityMode.NoMovingMode)
        {
            var oldBounds = HostedWindow.Bounds;
            var newBounds = new Rectangle(
            Pt.X + 8 - _CropLeft,
            Pt.Y + YShift - _CropTop,
            (int)_CacheWindowRect.Width + _CropLeft + _CropRight,
            (int)_CacheWindowRect.Height + _CropTop + _CropBottom
            );
            if (oldBounds != newBounds)
            {
                if (Check && WindowEx.ForegroundWindow == HostedWindow)
                {
                    await DetachAndDispose();
                    return;
                }
                else HostedWindow.Bounds = newBounds;
                if (ActivateCrop)
                    if (ForceInvalidateCrop || oldBounds.Size != newBounds.Size)
                    {
                        ForceInvalidateCrop = false;
                        _ = HostedWindow.SetRegionAsync(new(_CropLeft, _CropTop, HostedWindow.Bounds.Width - _CropLeft - _CropRight, HostedWindow.Bounds.Height - _CropTop - _CropBottom));
                    }
            }
        }
        if (CompatabilityMode.NoOwnerMode)
        {
            var oldBounds = HostedWindow.Bounds;
            if (new WindowRelative(HostedWindow).GetAboves().Take(10).Any(x => x == ParentWindow))
            {
                await Task.Delay(500);
                if (oldBounds == HostedWindow.Bounds && IsWindowVisible)
                {
                    HostedWindow.Activate(ActivationTechnique.SetWindowPosTopMost);
                    HostedWindow.Focus();
                }
            }
        }
        HostedWindow.IsVisible = true;
    }
}
