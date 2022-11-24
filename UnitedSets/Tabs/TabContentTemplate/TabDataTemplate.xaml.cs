using Microsoft.UI.Xaml;
using System;

namespace UnitedSets;

public partial class TabDataTemplate
{
    public readonly static Lazy<TabDataTemplate> Singleton = new(() => new());
    private TabDataTemplate()
    {
        InitializeComponent();
    }
    // Needed because x:Bind
}
