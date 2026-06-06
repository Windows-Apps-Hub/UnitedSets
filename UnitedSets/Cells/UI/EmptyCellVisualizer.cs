using UnitedSets.Apps;
using UnitedSets.Cells.Data;
using Windows.ApplicationModel.DataTransfer;

namespace UnitedSets.UI.Controls.Cells;

[QuickMarkup("""
    using Rectangle = Microsoft.UI.Xaml.Shapes.Rectangle;
    double CellMargin;
    private int SplitCount = 2;
    <root AllowDrop DragOver+=`OnDragOver` Drop+=`emptyCell.OnItemDrop`>
        <ScrollViewer
            Canvas.ZIndex=1
            HorizontalScrollMode=Auto VerticalScrollMode=Auto HorizontalScrollBarVisibility=Auto
            ZoomMode=Enabled
            MinZoomFactor=`0.1f` MaxZoomFactor=`1.5f`
        >
            <VStack Spacing=16
                CenterH CenterV
                DragOver+=`OnDragOver` Drop+=`emptyCell.OnItemDrop`
            >
                hintTb = <TextBlock CenterH FontSize=20 />
                <TextBlock CenterH FontSize=16 Text="or" />
                <Button CenterH @Click+=`emptyCell.Split(SplitCount, Orientation.Vertical)`> // intentionally swap orientation
                    <HStack Spacing=4>
                        <SymbolExIcon(GripperBarHorizontal) />
                        <TextBlock Text="Split Horizontally" />
                        <SymbolExIcon(GripperBarHorizontal) />
                    </HStack>
                </Button>
                <Button CenterH @Click+=`emptyCell.Split(SplitCount, Orientation.Horizontal)`> // intentionally swap orientation
                    <HStack Spacing=4>
                        <SymbolExIcon(GripperBarVertical) />
                        <TextBlock Text="Split Vertically" />
                        <SymbolExIcon(GripperBarVertical) />
                    </HStack>
                </Button>
                <HStack Spacing=8 CenterH>
                    <TextBlock CenterV Text="Number of Cells to Split:" />
                    plusbtn = <Button CenterV Padding=5 Content=<SymbolIcon(Add) /> @Click+=`SplitCount++;` />
                    <TextBlock CenterV Text=`SplitCount.ToString()` />
                    minusbtn = <Button CenterV Padding=5 Content=<SymbolIcon(Remove) /> @Click+=`SplitCount--;` />
                </HStack>
            </VStack>
        </ScrollViewer>
        rect = <Rectangle
            Margin=`new(CellMargin)`
            RadiusX=8 RadiusY=8
            StrokeThickness=3 Stroke=`Solid(Colors.Gray)`
            StrokeDashCap=Flat StrokeDashOffset=1.5 StrokeDashArray=`new() { 3 }`
        />
    </root>
    """)]
public partial class EmptyCellVisualizer : Grid
{
    private readonly EmptyCell emptyCell;

    public EmptyCellVisualizer(EmptyCell emptyCell)
    {
        this.emptyCell = emptyCell;
        Init();
        var transparent = Solid(Colors.Transparent);
        var layerBrushProp = ThemeResources.Get<Brush>("LayerFillColorDefaultBrush", this);
        emptyCell.HoverEffectProperty.ApplyAndRegisterForNewValue((_, hovering) =>
        {
            void Act()
            {
                if (hovering)
                {
                    rect.Fill = layerBrushProp.CurrentValue;
                    hintTb.Text = "Release mouse to drop window";
                }
                else
                {
                    rect.Fill = transparent;
                    hintTb.Text = "Hold CTRL and Drag Window Here";
                }
            }
            if (DispatcherQueue.HasThreadAccess)
                Act();
            else
                DispatcherQueue.TryEnqueue(Act);
        });
    }
    public void OnDragOver(object? _, DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!e.DataView.Properties.ContainsKey(Constants.UnitedSetsTabWindowDragProperty)) return;
        e.AcceptedOperation = DataPackageOperation.Move;
    }

}
