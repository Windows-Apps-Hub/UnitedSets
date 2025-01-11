using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Mvvm.Services;
using Get.EasyCSharp;
using Microsoft.UI.Windowing;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Animation;

namespace UnitedSets.UI.AppWindows;


public sealed partial class OOBEWindow : WinUIEx.WindowEx
{
    public UnitedSetsAppSettings Settings => UnitedSetsApp.Current.Settings;
    readonly bool IsInitialized = false;
    public OOBEWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        IsInitialized = true;
        AppWindow.Changed += WindowChanged;
        Page.Loaded += OOBEPageSetup;
        SystemBackdrop = new MicaBackdrop();
    }

    Storyboard LoadingAnimation => (Storyboard)Page.Resources[nameof(LoadingAnimation)];
    Storyboard LoadingAnimation2 => (Storyboard)Page.Resources[nameof(LoadingAnimation2)];
    SolidColorBrush SolidAccentColorBrush => (SolidColorBrush)Page.Resources[nameof(SolidAccentColorBrush)];
    LinearGradientBrush ShineBrush => (LinearGradientBrush)Page.Resources[nameof(ShineBrush)];
    [Event(typeof(RoutedEventHandler))]
    void OOBEPageSetup()
    {
        bar1.Value = (FlappyBird.SelectedIndex + 1);
        LoadingAnimation2.Stop();
        bar1.Foreground = ShineBrush;
        bar2.Foreground = SolidAccentColorBrush;
        LoadingAnimation.Begin();
        bar2.Value = 0;
    }

    [Event(typeof(SelectionChangedEventHandler))]
    private void FlipViewPageChanged()
    {
        if (!IsInitialized) return;
        var progress = FlappyBird.SelectedIndex + 1;
        Steps.Text = $"Step {progress}/ 4";
        if (progress <= 2)
        {
            bar1.Value = progress;
            LoadingAnimation2.Stop();
            bar1.Foreground = ShineBrush;
            bar2.Foreground = SolidAccentColorBrush;
            LoadingAnimation.Begin();
            bar2.Value = 0;
        }
        else
        {
            LoadingAnimation.Stop();
            bar2.Foreground = ShineBrush;
            bar1.Foreground = SolidAccentColorBrush;
            LoadingAnimation2.Begin();
            bar2.Value = progress - 2;
            if (FlappyBird.SelectedIndex == 3)
            {
                LoadingAnimation2.Stop();
                bar2.Foreground = SolidAccentColorBrush;
            }
        }

    }

    [Event(typeof(TypedEventHandler<AppWindow, AppWindowChangedEventArgs>))]
    private void WindowChanged(AppWindowChangedEventArgs args)
    {
        if (args.DidSizeChange)
        {
            if (Bounds.Height > 700)
                Stepper.Visibility = Visibility.Visible;
            else
                Stepper.Visibility = Visibility.Collapsed;
        }
    }
    [RelayCommand]
    private void CompleteOOBE()
    {
        AppWindow.Hide();
        (Application.Current as App)!.LaunchNewMain();
    }
}
