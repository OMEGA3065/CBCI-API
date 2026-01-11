using CustomItemLib.Compat.RaCustomMenu;
using LabApi.Features.Console;
using RaCustomMenu.API;

namespace CustomItemLib.Compat;

internal static class RaCustomMenuCompat
{
    private static bool HasInicialized = false;
    public static void Init()
    {
        if (CustomItemLibPlugin.Instance?.RaCustomMenuFound != true) return;
        if (HasInicialized) return;
        Provider.RegisterProvider(new ItemNamespaceProvider());
        HasInicialized = true;
    }
}