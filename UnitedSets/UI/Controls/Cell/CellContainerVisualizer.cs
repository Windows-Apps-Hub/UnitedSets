using Get.UI.Data;
using Get.Data.Bindings.Linq;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
using Get.Data.XACL;
using Get.Data.Properties;
using Get.Data.DataTemplates;
using Microsoft.UI.Xaml;
namespace UnitedSets.UI.Controls.Cell;
public partial class CellContainerVisualizer(ContainerCell cellContainer) : TemplateControl<Grid>
{
    protected override void Initialize(Grid rootElement)
        => rootElement.Children.Add(new OrientedStack
        {
            Tag = "Cell Container",
            OrientationBinding = OneWay(cellContainer.OrientationProperty),
            Children =
            {
                CollectionItemsBinding.Create(
                    cellContainer.SubCells,
                    new DataTemplate<UnitedSets.Classes.Cell, UIElement>(
                        x => new GenericCellVisualizer(x.CurrentValue)
                        {
                            CellBinding = OneWay(x)
                        }
                    )
                )
            }
        });
}
