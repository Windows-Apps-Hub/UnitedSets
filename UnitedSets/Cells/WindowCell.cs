using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Get.Data.Properties;
using static Get.Data.Properties.AutoTyper;
using WindowHoster;
using Get.Data.Collections;

namespace UnitedSets.Cells;
public partial class WindowCell : Cell
{
    public RegisteredWindow Window { get; }

    public WindowCell(ContainerCell Parent, RegisteredWindow window) : base(Parent)
    {
        Window = window;
        window.Closed += GoToEmpty;
        window.Detached += GoToEmpty;
        if (!window.IsValid)
            GoToEmpty();
    }
    bool wentToEmpty = false;
    async void GoToEmpty()
    {
    restart:
        if (wentToEmpty)
            return;
        var idx = Parent!.SubCells.IndexOf(this);
        if (idx < 0)
        {
            await Task.Delay(1000);
            goto restart;
        }
        wentToEmpty = true;
        Parent.SubCells[idx] = new EmptyCell(Parent);
    }
}
