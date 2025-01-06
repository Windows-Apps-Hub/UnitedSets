using System.Threading.Tasks;
using UnitedSets.Configurations;

namespace UnitedSets;

class UnitedSetsAppConfiguration
{
    public UnitedSetsAppConfiguration()
    {
        PersistantService = new();
        if (!PersistantService.LoadPreviousSessionData(out var cfg))
            PersistantService.LoadInitialSettingsAndTheme(out cfg);
        MainConfiguration = cfg;
    }
    public PreservedTabDataService PersistantService { get; }
    public USConfig MainConfiguration { get; private set; } = null!;
    public void SaveCurSettingsAsDefault() => PersistantService.ExportSettings(USConfig.DefaultConfigFile, true, true);//don't give user any choice as to what for now so will exclude current tabs
    public async Task ResetSettingsToDefault()
    {
        await PersistantService.ResetSettings();
        SaveCurSettingsAsDefault();
    }
    public void SaveCurrentSession()
    {
        PersistantService.ExportSettings(USConfig.SessionSaveConfigFile, true, ExcludeTabs: false);
    }
}
