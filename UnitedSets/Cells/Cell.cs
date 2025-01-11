using System.ComponentModel;
using Get.Data.Properties;
using UnitedSets.Tabs;
namespace UnitedSets.Cells;
[AutoProperty]
public abstract partial class Cell(ContainerCell? Parent) : INotifyPropertyChanged
{
    public ContainerCell? Parent { get; } = Parent;

    public IProperty<double> RelativeSizeProperty { get; } = Auto(1d);
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void NotifyPropertyChanged(string PropertyName)
        => PropertyChanged?.Invoke(this, new(PropertyName));
    public IProperty<bool> HoverEffectProperty { get; } = Auto(false);
}
