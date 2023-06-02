
using CommunityToolkit.Mvvm.Input;

namespace UnitedSets.Classes.Tabs;

partial class TabBase
{
    [RelayCommand]
    public async void TryCloseNoWait() => await TryCloseAsync();
    [RelayCommand]
    void DetachWindow() => DetachAndDispose(JumpToCursor: false);
}
