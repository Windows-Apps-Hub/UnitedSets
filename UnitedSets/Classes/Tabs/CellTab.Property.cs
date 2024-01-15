using Get.EasyCSharp;

namespace UnitedSets.Classes.Tabs;

partial class CellTab
{
    [Property(SetVisibility = GeneratorVisibility.DoNotGenerate, OverrideKeyword = true)]
    bool _IsDisposed;

    [Property(OnChanged = nameof(OnMainCellChanged))]
    public Cell _MainCell;
}
