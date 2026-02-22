namespace UnitedSets.UI.Controls.MainArea;

[QuickMarkup("""
    <root>
        <.RowDefinitions>
            <RowDefinition />
            <RowDefinition />
        </.RowDefinitions>
        TabViewBorder = <Border MinHeight=40 PointerMoved="DragRegion_PointerMoved" Background="Transparent">
            <guics:DragRegion x:Name="DragRegion" Background="Transparent" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
                <guics:DragRegion.ColumnDefinitions>
                    <ColumnDefinition Width="{x:Bind GridLengthFromPixelInt(AppWindow.TitleBar.LeftInset), Mode=OneTime}"/>
                    <ColumnDefinition/>
                    <ColumnDefinition Width="32"/>
                    <ColumnDefinition Width="{x:Bind GridLengthFromPixelInt(AppWindow.TitleBar.RightInset), Mode=OneTime}"/>
                </guics:DragRegion.ColumnDefinitions>
                <controls:HorizontalTabs
                    Grid.Column="1"
                    x:Name="TabView"
                    x:FieldModifier="public"
                    HorizontalAlignment="Left"
                    VerticalAlignment="Center"
                    SelectionChanged="TabSelectionChanged"
                >
                    <controls:HorizontalTabs.TabStripHeader>
                        <controls:MainWindowControlButton
                            VerticalAlignment="Center"
                            Margin="5,0,0,0"
                            Style="{ThemeResource ToolbarButton}"
                            MainWindow="{x:Bind}"
                        />
                    </controls:HorizontalTabs.TabStripHeader>
                    <controls:HorizontalTabs.TabStripFooter>
                        <controls:AddTabSplitButton
                            Canvas.ZIndex="8"
                            Margin="0,0,20,0"
                            HorizontalAlignment="Left"
                            guics:DragRegion.Clickable="True"
                            AddTab="OnAddTabButtonClick" AddSplitableTab="AddSplitableTab"
                        />
                    </controls:HorizontalTabs.TabStripFooter>
                </controls:HorizontalTabs>
            </guics:DragRegion>
        </Border>
        <Border Grid.Row="1" x:Name="MainAreaBorder">
            <controls:TabVisualizer
                Tab="{x:Bind us:UnitedSetsApp.Current.SelectedTab, Mode=OneWay}"
                HorizontalContentAlignment="Stretch"
                VerticalContentAlignment="Stretch"
            />
        </Border>
    </root>
    """)]
partial class HorizontalTabsPanel : Grid
{
}
