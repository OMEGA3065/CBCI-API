using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Console;

namespace CustomItemLib.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(MagazineModule), nameof(MagazineModule.AmmoMax), MethodType.Getter)]
public static class FirearmMaxAmmo
{
    public static readonly Dictionary<ushort, int> MaxAmmoOverrides = [];

    public static void Postfix(MagazineModule __instance, ref int __result)
    {
        if (!MaxAmmoOverrides.TryGetValue(__instance.Item.ItemSerial, out var maxAmmo))
            return;
        __result = maxAmmo;
    }
}