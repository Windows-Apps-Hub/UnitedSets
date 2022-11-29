using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnitedSets.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OOBEPage : Page
    {
        public SettingsService Settings = App.Current.Services.GetService<SettingsService>() ?? throw new NullReferenceException();
        public OOBEPage()
        {
            this.InitializeComponent();
            bar1.Value = (FlappyBird.SelectedIndex + 1) * 10;
            LoadingAnimation2.Stop();
            bar1.Foreground = ShineBrush;
            bar2.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
            LoadingAnimation.Begin();
            bar2.Value = 0;
        }


        private void FlappyBird_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Steps.Text = "Step " + (FlappyBird.SelectedIndex + 1) + "/4";
                if (FlappyBird.SelectedIndex <= 1)
                {
                    // Section.Text = "Feature showcase";
                    bar1.Value = (FlappyBird.SelectedIndex + 1) * 10;
                    LoadingAnimation2.Stop();
                    bar1.Foreground = ShineBrush;
                    bar2.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
                    LoadingAnimation.Begin();
                    bar2.Value = 0;
                }
                else
                {
                    LoadingAnimation.Stop();
                    bar2.Foreground = ShineBrush;
                    bar1.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
                    LoadingAnimation2.Begin();
                    //  Section.Text = "Setup preferences";
                    bar2.Value = (FlappyBird.SelectedIndex - 1) * 10;
                    if (FlappyBird.SelectedIndex == 3)
                    {
                        LoadingAnimation2.Stop();
                        bar2.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
                    }
                }
            }
            catch
            {

            }

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Height > 700)
                Stepper.Visibility = Visibility.Visible;
            else
                Stepper.Visibility = Visibility.Collapsed;
        }
        // TEMPORARY
        private void CompletedOOBE_Click(object sender, RoutedEventArgs e)
        {
            App.Current.o_window?.Hide();
            App.Current.LaunchNewMain();
        }
    }
}
