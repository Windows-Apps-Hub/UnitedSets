using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UnitedSets.Classes;

namespace UnitedSets;

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
//public class CellContentTemplateSelector : DataTemplateSelector
//{
//    //static TabDataTemplate DataTemplates => TabDataTemplate.Singleton.Value;
//    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
//    {
//        return SelectTemplateCore(item);
//    }
//    protected override DataTemplate SelectTemplateCore(object item)
//    {
//        //if (item is not Cell cell) return base.SelectTemplateCore(item);
//        //if (cell.Empty)
//        //    return DataTemplates.EmptyCellTabDataTemplate;
//        return base.SelectTemplateCore(item);
//    }
//}
