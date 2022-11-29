using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;

namespace UnitedSets.Helpers;

public class MicaHelper
{
    static bool IsMicaInfinite = true;
    WindowsSystemDispatcherQueueHelper? m_wsdqHelper;
    MicaController? m_micaController;
    SystemBackdropConfiguration? m_configurationSource;
    Window? window;

    public bool TrySetMicaBackdrop(Window _window)
    {
        window = _window;
        if (MicaController.IsSupported())
        {
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            // Hooking up the policy object
            m_configurationSource = new SystemBackdropConfiguration();
            window.Activated += Window_Activated;
            window.Closed += Window_Closed;

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;

            m_micaController = new MicaController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_micaController.AddSystemBackdropTarget(window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (m_configurationSource == null) return;
        bool IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        if (IsInputActive)
            m_configurationSource.IsInputActive = true;
        else if (!IsMicaInfinite)
            m_configurationSource.IsInputActive = false;
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (m_micaController != null)
        {
            m_micaController.Dispose();
            m_micaController = null;
        }
        if (window is not null)
            window.Activated -= Window_Activated;
        m_configurationSource = null;
    }
}
