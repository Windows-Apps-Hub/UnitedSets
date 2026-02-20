using UnitedSets.Cells;

namespace UnitedSets.Tabs;

partial class CellTab
{
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate, OverrideKeyword = true)]
    bool _IsDisposed;

    public IProperty<ContainerCell> MainCellProperty { get; }
}
