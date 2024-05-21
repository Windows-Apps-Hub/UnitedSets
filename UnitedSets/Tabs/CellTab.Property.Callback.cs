namespace UnitedSets.Tabs;

partial class CellTab
{

    void OnMainCellChanged()
        => InvokePropertyChanged(nameof(MainCell));
}
