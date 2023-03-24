using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;
using UnitedSets.Classes.Tabs;

namespace UnitedSets.Templates;

public class TabBaseContentTemplateSelector : DataTemplateSelector
{
    static readonly TabDataTemplate DataTemplates = TabDataTemplate.Singleton.Value;
    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    protected override DataTemplate SelectTemplateCore(object item)
    {
        return item switch
        {
            HwndHostTab => DataTemplates.SingleTabDataTemplate,
            CellTab => DataTemplates.CellTabDataTemplate,
            _ => base.SelectTemplateCore(item)
        };
    }
}