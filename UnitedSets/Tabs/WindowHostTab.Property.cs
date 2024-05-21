using WindowEx = WinWrapper.Windowing.Window;
using WindowHoster;
namespace UnitedSets.Tabs;

partial class WindowHostTab
{
    public readonly WindowEx Window;

    public RegisteredWindow RegisteredWindow { get; }

  //  [Property(OverrideKeyword = true, OnChanged = nameof(OnSelectedChanged))]
  //  bool _Selected;
  //  void OnSelectedChanged()
  //  {
		////HwndHost.SetVisible(_Selected);
		//InvokePropertyChanged(nameof(Selected));
		
	//}

    
}
