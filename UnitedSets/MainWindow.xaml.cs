using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WinRT.Interop;

namespace UnitedSets;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    ObservableCollection<HwndHostTab> Tabs { get; } = new();
    readonly WindowEx WindowEx;
   // readonly AppWindow AppWindow;
    public MainWindow()
    {
        InitializeComponent();
        WindowEx = WindowEx.FromWindowHandle(WindowNative.GetWindowHandle(this));
        var paint = new HwndHostTab(this, WindowEx.GetAllWindows().First(x => x.Text.Contains("Paint")).Root);
        paint.Tempicon = new BitmapImage(new Uri("https://media.discordapp.net/attachments/757560235144642577/1030621242975342612/unknown.png"));
        Tabs.Add(paint);
        paint.Closed += () => Tabs.Remove(paint);
        var vscode = new HwndHostTab(this, WindowEx.GetAllWindows().First(x => x.Text.Contains("Notepad")).Root);
        vscode.Tempicon = new BitmapImage(new Uri("https://media.discordapp.net/attachments/757560235144642577/1030621196972216421/unknown.png"));
        Tabs.Add(vscode);
        vscode.Closed += () => Tabs.Remove(vscode);
    }
}
