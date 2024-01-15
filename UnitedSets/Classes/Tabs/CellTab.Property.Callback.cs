namespace UnitedSets.Classes.Tabs;

partial class CellTab
{

    void OnMainCellChanged()
        => InvokePropertyChanged(nameof(MainCell));
}
