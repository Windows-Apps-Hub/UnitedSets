using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Linq;
using CommunityToolkit.WinUI.UI;

namespace UnitedSets;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        //var AllProperties = (
        //    from prop in typeof(Microsoft.UI.Xaml.Controls.Button).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy).AsEnumerable()
        //    let x = prop.GetValue(null)
        //    where x is DependencyProperty
        //    select (prop.Name, (DependencyProperty)x)
        //).ToArray();
        //Style S = (Style)Resources["WindowCaptionButton"];
        //foreach (var setterbase in S.Setters)
        //{
        //    var setter = (Setter)setterbase;
        //    var properties = setter.Property;
        //    var output = AllProperties.First(x => x.Item2 == properties);
        //    var value = setter.Value;
        //    if (value is ControlTemplate template)
        //    {
        //        // I want to get XAML representation of template
        //    }
        //}
        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window? m_window;
}
