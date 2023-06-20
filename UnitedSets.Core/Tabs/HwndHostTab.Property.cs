using WindowEx = WinWrapper.Windowing.Window;
using EasyCSharp;
namespace UnitedSets.Classes.Tabs;

partial class HwndHostTab
{
    public readonly WindowEx Window;

    public OurHwndHost HwndHost { get; }

    [Property(OverrideKeyword = true, OnChanged = nameof(OnSelectedChanged))]
    bool _Selected;
    void OnSelectedChanged()
    {
		HwndHost.SetVisible(_Selected);
		InvokePropertyChanged(nameof(Selected));
		
	}

    
}
