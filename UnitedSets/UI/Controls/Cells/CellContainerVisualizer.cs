using Get.Data.XACL;
using Get.Data.DataTemplates;
using UnitedSets.Cells;
namespace UnitedSets.UI.Controls.Cells;
[AutoProperty]
public partial class CellContainerVisualizer(ContainerCell cellContainer) : TemplateControl<Grid>
{
    public IProperty<double> CellMarginProperty { get; } = Auto(10d);
    protected override void Initialize(Grid rootElement)
        => rootElement.Children.Add(new OrientedStack
        {
            Tag = "Cell Container",
            OrientationBinding = OneWay(cellContainer.OrientationProperty),
            Children =
            {
                CollectionItemsBinding.Create(
                    cellContainer.SubCells,
                    new DataTemplate<Cell, UIElement>(
                        x => new GenericCellVisualizer(x.CurrentValue)
                        {
                            CellBinding = OneWay(x),
                            CellMarginBinding = OneWay(CellMarginProperty)
                        }
                    )
                )
            }
        });
}
