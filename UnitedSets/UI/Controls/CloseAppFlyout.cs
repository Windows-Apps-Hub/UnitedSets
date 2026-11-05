using UnitedSets.Mvvm.Services;

namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    using UnitedSets.Apps;
    <setup>
    Reference<Color> SystemAccentColorLight2 = null!;
    </setup>
    <root Placement=BottomEdgeAlignedRight !ShouldConstrainToRootBounds>
        <StackPanel Orientation=Vertical Spacing=16>
            <TextBlock CenterH Text="What should happen to the windows in this set?" />
            <StackPanel Orientation=Horizontal Spacing=16 CenterH>
                <Button `ToolBarBtn`
                    `x => SystemAccentColorLight2 = ThemeResources.Get<Color>("SystemAccentColorLight2", x).CreateReadOnlyReference()`
                    @Click+=`Choose(UnitedSetsCloseMode.ReleaseWindow);`
                    Foreground=`Solid(SystemAccentColorLight2.Value)`
                    BorderBrush=`Solid(SystemAccentColorLight2.Value)`
                    Content="Keep apps open" />
                <Button `ToolBarBtn`
                    @Click+=`Choose(UnitedSetsCloseMode.SaveCloseWindow);`
                    Visibility=`Constants.VisibleOnExperimental`
                    Content="Save and close"
                />
                <Button `ToolBarBtn` @Click+=`Choose(UnitedSetsCloseMode.CloseWindow);`
                    Foreground=`Solid(CloseRed)` BorderBrush=`Solid(CloseRed)`
                    Content="Close apps"
                 />
            </StackPanel>
            RememberOption = <CheckBox Content="Remember my option (can be changed in the settings)" CenterH />
        </StackPanel>
    </root>
    """)]
partial class CloseAppFlyout : BackdropedFlyout
{
    static readonly Color CloseRed = Color.FromArgb(255, 0xe9, 0x6e, 0x60);
    public event Action<UnitedSetsCloseMode>? CloseRequest;
    void Choose(UnitedSetsCloseMode mode)
    {
        // "Save and close" saves a session, it's not something to do every time
        if (RememberOption.IsChecked is true && mode is not UnitedSetsCloseMode.SaveCloseWindow)
            UnitedSetsApp.Current.Settings.CloseSetWithTabsBehavior.Value = mode is UnitedSetsCloseMode.ReleaseWindow
                ? CloseSetWithTabsBehaviors.ReleaseWindows
                : CloseSetWithTabsBehaviors.CloseWindows;
        RememberOption.IsChecked = false;
        CloseRequest?.Invoke(mode);
    }
    static void ToolBarBtn(Button btn)
    {
        btn.CenterH();
        btn.Background = Solid(Colors.Transparent);
    }
}
