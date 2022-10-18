using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using WinRT.Interop;
using WinUIEx;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI;
using Microsoft.UI;
using Windows.ApplicationModel.DataTransfer;
using System.Numerics;
using Microsoft.UI.Xaml.Media;
using System;
using WindowEx = WinWrapper.Window;
using UnitedSets.Classes;

namespace UnitedSets;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    public ObservableCollection<HwndHostTab> Tabs { get; } = new();
    readonly WindowEx WindowEx;
    public MainWindow()
    {
        Title = "UnitedSets";
        InitializeComponent();
        WindowEx = WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this));
        //var paint = new HwndHostTab(this, WindowEx.GetAllWindows().First(x => x.Text.Contains("Paint")).Root);
        //paint.Tempicon = new BitmapImage(new Uri("https://media.discordapp.net/attachments/757560235144642577/1030621242975342612/unknown.png"));
        //Tabs.Add(paint);
        //paint.Closed += () => Tabs.Remove(paint);
        //var vscode = new HwndHostTab(this, WindowEx.GetAllWindows().First(x => x.Text.Contains("Notepad")).Root);
        //vscode.Tempicon = new BitmapImage(new Uri("https://media.discordapp.net/attachments/757560235144642577/1030621196972216421/unknown.png"));
        //Tabs.Add(vscode);
        //vscode.Closed += () => Tabs.Remove(vscode);
        Closed += delegate
        {

        };
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(CustomDragRegion);
    }
    readonly AddTabFlyout AddTabFlyout = new();
    private async void AddTab(TabView sender, object args)
    {
        this.Hide();
        await AddTabFlyout.ShowAtCursorAsync();
        this.Show();
        var result = AddTabFlyout.Result;
        if (!result.IsValid) 
            return;
        result = result.Root;
        if (result.Handle == IntPtr.Zero) 
            return;
        if (result.Handle == AddTabFlyout.GetWindowHandle()) 
            return;
        if (result.Handle == WindowEx.Handle) 
            return;
        if (Tabs.FirstOrDefault(x => x.Window.Handle == result.Handle) is not null) 
            return;
        var newTab = new HwndHostTab(this, result);
        Tabs.Add(newTab);
        TabView.SelectedItem = newTab;
    }

    private void TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
    {
        if (args.Tab.Tag is HwndHostTab Tab)
        {
            Tab.DetachAndDispose();
        }
    }

    private void TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
    {
        var firstItem = args.Tab;
        args.Data.Properties.Add("UnitedSetsTab", firstItem);
        args.Data.RequestedOperation = DataPackageOperation.Move;
    }
}
