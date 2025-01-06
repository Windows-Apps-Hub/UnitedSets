using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Configurations;
using Windows.System;
using CNTRLS = Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnitedSets.UI.AppWindows;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ExportImportInputPage : Page
{
    public ExportImportInputPage()
    {
        this.InitializeComponent();
        DataContext = new ExportImportInputViewModel();
        Loaded += ExportImportInputPage_Loaded;

    }



    private void ExportImportInputPage_Loaded(object sender, RoutedEventArgs e)
    {
        txtFile.Focus(FocusState.Keyboard);
    }

    public ExportImportInputViewModel vm => (ExportImportInputViewModel)DataContext;

    public static async Task<ExportImportInputViewModel?> ShowExportImport(bool ForExportNotImport, MainWindow parentWindow)
    {
        var origSelected = UnitedSetsApp.Current.SelectedTab;
        UnitedSetsApp.Current.SelectedTab = null;
        var wind = new ExportImportInputPage();
        wind.vm.SaveNotLoad = ForExportNotImport;
        wind.vm.hWnd = WinRT.Interop.WindowNative.GetWindowHandle(parentWindow);
        var dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = parentWindow.Content.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Import Export Options";


        void ShowFlyout(string msg, ContentDialogButtonClickEventArgs? args = null)
        {
            //	var dlg2 = new ContentDialog();
            //	dlg2.XamlRoot = parentWindow.Content.XamlRoot;
            //dlg2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            //	dlg2.Content = new TextBlock { Text=msg};
            //	dlg2.Title = "Confirm";
            //	if (args != null) {
            //		dlg2.PrimaryButtonText = "Yes";
            //		dlg2.CloseButtonText = "Cancel";
            //	} else {
            //		dlg2.CloseButtonText = "OK";
            //	}
            //		var deferral = args != null ? args.GetDeferral() : null;
            //	var res = await dlg2.ShowAsync();
            //	if (res != ContentDialogResult.Primary && args != null)
            //		args.Cancel = true;

            //	if (deferral != null)
            //		deferral.Complete();
            var flyout = new CNTRLS.Flyout();
            var sp = new StackPanel { Spacing = 10 };
            sp.Children.Add(new TextBlock { Text = msg });
            var btn = new Button() { Content = "OK" };
            btn.Click += (_, _) => flyout.Hide();
            sp.Children.Add(btn);
            flyout.Content = sp;
            flyout.ShowAt(wind.txtFile);//because we don't have the save button to do
        }
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.IsSecondaryButtonEnabled = false;
        dialog.DefaultButton = ContentDialogButton.None;//as it doesn't like to trigger click for hgitting enter


        var primaryBtnActionShouldCancel = () =>
        {

            if (String.IsNullOrWhiteSpace(wind.vm.FullFilename))
                return false;
            var fileExists = File.Exists(wind.vm.FullFilename);

            if (ForExportNotImport && fileExists)
            {
                //ShowFlyout("Do you want to override the existing file?",args);
                //nvm we will let the file save dialog do it, and if they type it in hope they know if it does or not;00
            }
            else if (!ForExportNotImport && !fileExists)
            {
                ShowFlyout("File does not exist, try again or hit cancel");
                return true;
            }
            return false;

        };
        dialog.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                if (!primaryBtnActionShouldCancel())
                {
                    wind.vm.OverrideAsSuccess = true;
                    dialog.Hide();
                }

            }
        };
        dialog.PrimaryButtonClick += (_, args) => args.Cancel = primaryBtnActionShouldCancel();
        dialog.Content = wind;
        dialog.PrimaryButtonText = wind.vm.AcceptFileButtonText;

        dialog.CloseButtonText = "Cancel";
        wind.vm.RequestClose += (_, _) => dialog.Hide();

        var result = await dialog.ShowAsync();
        UnitedSetsApp.Current.SelectedTab = origSelected;
        if ((result == ContentDialogResult.Primary || wind.vm.OverrideAsSuccess) && String.IsNullOrWhiteSpace(wind.vm.FullFilename) == false)
        {
            var fileExists = File.Exists(wind.vm.FullFilename);
            if (!ForExportNotImport && !fileExists)
                throw new FileNotFoundException(wind.vm.FullFilename);


            return wind.vm;
        }
        return null;

    }

}
