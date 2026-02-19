namespace UnitedSets.Tabs;

public partial class TabGroup(string Name) : INotifyPropertyChanged
{
    [AutoNotifyProperty]
    string _Name = Name;

    public ObservableCollection<TabBase> Tabs { get; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

}
