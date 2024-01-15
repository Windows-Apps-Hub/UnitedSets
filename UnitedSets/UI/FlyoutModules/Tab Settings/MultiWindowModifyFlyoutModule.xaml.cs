using Get.EasyCSharp;
using Microsoft.UI.Xaml.Controls;
using WindowHoster;
namespace UnitedSets.UI.FlyoutModules;

public sealed partial class MultiWindowModifyFlyoutModule
{
    public MultiWindowModifyFlyoutModule(RegisteredWindow[] Windows)
    {
        this.RegisteredWindows = Windows;
        InitializeComponent();
        foreach (var registeredWindow in Windows)
        {
            RegisteredWindowSelector.Items.Add(registeredWindow.Window.TitleText);
        }
    }
    readonly RegisteredWindow[] RegisteredWindows;

    [Event(typeof(SelectionChangedEventHandler))]
    void RegisteredWindowSelector_SelectionChanged()
    {
        if (RegisteredWindowSelector.SelectedIndex != -1)
            ModifyWindowFlyoutModulePlace.Child = new ModifyWindowFlyoutModule(
                RegisteredWindows[RegisteredWindowSelector.SelectedIndex]
            );
        else
            ModifyWindowFlyoutModulePlace.Child = null;
    }
}
