namespace UnitedSets.Classes.Tabs;

partial class CellTab
{
    void OnSelectedChanged()
    {
        _MainCell.IsVisible = _Selected;
        InvokePropertyChanged(nameof(Selected));
        InvokePropertyChanged(nameof(Visibility));
    }

    void OnMainCellChanged()
        => InvokePropertyChanged(nameof(MainCell));
}
