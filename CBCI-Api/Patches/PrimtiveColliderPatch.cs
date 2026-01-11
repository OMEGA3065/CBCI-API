namespace CustomItemLib.Patches
{
    using AdminToys;
    using HarmonyLib;

    [HarmonyPatch(typeof(PrimitiveObjectToy), nameof(PrimitiveObjectToy.SetFlags))]
    public static class PrimtiveColliderPatch
    {
        public static void Postfix(PrimitiveObjectToy __instance)
        {
            if (((int)__instance.PrimitiveFlags & 4) == 0) return;
            __instance._collider.enabled = true;
        }
    }
}