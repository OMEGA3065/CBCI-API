using LabApi.Features.Console;
using RaCustomMenuLabApi.API;

namespace CustomItemLib.Compat;

internal static class RaCustomMenuCompat
{
    public static void Init()
    {
        if (CustomItemLibPlugin.Instance?.RaCustomMenuFound != true) return;
        Provider.RegisterAllProviders();
    }
}