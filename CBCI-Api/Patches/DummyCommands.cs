namespace CustomItemLib.Patches
{
    using System.Text;
    using CommandSystem;
    using CommandSystem.Commands.RemoteAdmin.Dummies;
    using HarmonyLib;
    using LabApi.Features.Permissions;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using RemoteAdmin;
    using RemoteAdmin.Communication;
    using Utils;

    public static class LastDummyCommandIssuer
    {
        public static Player Player = null;
    }

    [HarmonyPatch(typeof(ActionDummyCommand), nameof(ActionDummyCommand.Execute))]
    public static class PatchActionDummyExecute
    {
        [HarmonyPatch, HarmonyPrefix]
        public static bool Prefix(RaDummyActions __instance, ArraySegment<string> arguments, ICommandSender sender)
        {
            LastDummyCommandIssuer.Player = Player.Get(sender);
            return true;
        }

        // [HarmonyPatch, HarmonyPostfix]
        // public static bool Postfix(RaDummyActions __instance, ArraySegment<string> arguments, ICommandSender sender)
        // {
        //     if (Instance.LastDummyCommandIssuer is null)
        //     {
        //         Instance.LastDummyCommandIssuer = Player.Get(sender);
        //         return true;
        //     }
        //     return false;
        // }
    }
}