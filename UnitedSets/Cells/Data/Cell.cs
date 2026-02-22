namespace UnitedSets.Cells.Data;

[AutoProperty]
[QuickMarkup("""
    double RelativeSize = 1;
    """)]
public abstract partial class Cell(ContainerCell? Parent)
{
    public ContainerCell? Parent { get; } = Parent;
    // due to multi-threading, needs to be a non ref version
    public IProperty<bool> HoverEffectProperty { get; } = Auto(false);
}
