namespace UnitedSets.UI.Controls;

[QuickMarkup("""
    using UnitedSets.Apps;
    <setup>
    Reference<Color> SystemAccentColorLight2 = null!;
    </setup>
    <root Placement=BottomEdgeAlignedRight !ShouldConstrainToRootBounds>
        <VStack Spacing=16>
            <TextBlock CenterH Text="How would you like to close the app?" />
            <HStack Spacing=16 CenterH>
                <Button `ToolBarBtn`
                    `x => SystemAccentColorLight2 = ThemeResources.Get<Color>("SystemAccentColorLight2", x).CreateReadOnlyReference()`
                    @Click+=`CloseRequest?.Invoke(UnitedSetsCloseMode.ReleaseWindow);`
                    Foreground=`Solid(SystemAccentColorLight2.Value)`
                    BorderBrush=`Solid(SystemAccentColorLight2.Value)`
                    Content="Release" />
                <Button `ToolBarBtn`
                    @Click+=`CloseRequest?.Invoke(UnitedSetsCloseMode.SaveCloseWindow);`
                    Visibility=`Constants.VisibleOnExperimental`
                    Content="Save and close"
                />
                <Button `ToolBarBtn` @Click+=`CloseRequest?.Invoke(UnitedSetsCloseMode.CloseWindow);`
                    Foreground=`Solid(CloseRed)` BorderBrush=`Solid(CloseRed)`
                    Content="Close"
                 />
            </HStack>
            <CheckBox Content="Remember my option" CenterH />
        </VStack>
    </root>
    """)]
partial class CloseAppFlyout : BackdropedFlyout
{
    static readonly Color CloseRed = Color.FromArgb(255, 0xe9, 0x6e, 0x60);
    public event Action<UnitedSetsCloseMode>? CloseRequest;
    static void ToolBarBtn(Button btn)
    {
        btn.CenterH();
        btn.Background = Solid(Colors.Transparent);
    }
}
