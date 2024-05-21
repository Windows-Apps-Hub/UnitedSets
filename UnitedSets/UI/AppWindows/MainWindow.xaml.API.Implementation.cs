using Microsoft.UI.Xaml.Controls;
using System.Linq;
using WinUIEx;
using System;
using WindowEx = WinWrapper.Windowing.Window;
using UnitedSets.Classes;
using System.ComponentModel;
using UnitedSets.Tabs;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowHoster;

namespace UnitedSets.UI.AppWindows;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    private partial async Task TimerStop()
    {
        timer.Stop();
        OnUIThreadTimerLoop();
        await Task.Delay(100);
    }
    public void SaveCurSettingsAsDefault() => persistantService.ExportSettings(USConfig.DefaultConfigFile, true, true);//don't give user any choice as to what for now so will exclude current tabs
    public async Task ResetSettingsToDefault()
    {
        await persistantService.ResetSettings();
        SaveCurSettingsAsDefault();
    }
}
