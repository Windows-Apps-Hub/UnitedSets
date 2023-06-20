using EasyCSharp;

namespace UnitedSets.Classes.Tabs;

partial class CellTab
{
    [Property(OnChanged = nameof(OnSelectedChanged), OverrideKeyword = true)]
    bool _Selected;

    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate, OverrideKeyword = true)]
    bool _IsDisposed;

    [Property(OnChanged = nameof(OnMainCellChanged))]
    public Cell _MainCell;
}
