namespace UnitedSets.Cells;

[AutoProperty]
public abstract partial class Cell(ContainerCell? Parent)
{
    public ContainerCell? Parent { get; } = Parent;
    public IProperty<double> RelativeSizeProperty { get; } = Auto(1d);
    public IProperty<bool> HoverEffectProperty { get; } = Auto(false);
}
