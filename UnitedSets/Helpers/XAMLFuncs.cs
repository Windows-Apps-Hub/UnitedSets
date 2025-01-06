using Get.Symbols;
using Microsoft.UI.Xaml.Controls;

namespace UnitedSets.Helpers;

static class XAMLFuncs
{
    public static string ConcatWithSpaceIf(bool b, string A, string B)
    {
        if (b)
            return $"{A} {B}";
        else return A;
    }
    public static Symbol ToSymbol(SymbolEx symbolEx)
        => (Symbol)(int)symbolEx;
}
