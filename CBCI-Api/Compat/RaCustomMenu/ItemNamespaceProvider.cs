using CommandSystem;
using CustomItemLib.API;
using CustomItemLib.Patches;
using LabApi.Features.Console;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using NetworkManagerUtils.Dummies;
using NorthwoodLib.Pools;
using RaCustomMenu.API;

namespace CustomItemLib.Compat.RaCustomMenu;

/// <summary>
/// Provides the RaCustomMenu functionality allowing easier giving of items.
/// </summary>
public class ItemNamespaceProvider : Provider
{
    public override string CategoryName => "CBCI Give Item";
    public override bool IsDirty { get; } = true;

    private static readonly List<string> NamespacesRegistered = [];

    private static void RegisterNameSpace(string pluginNamespace)
    {
        Provider.RegisterDynamicProvider($"{pluginNamespace}", true, referenceHub =>
        {
            List<DummyAction> list = [new DummyAction("<color=red>[CLOSE]</color>", () =>
            {
                Provider.UnregisterDynamicProvider($"{pluginNamespace}");
            })];
            foreach (var ci in CustomItemManager.items)
            {
                if (ci.Key.PluginNamespace != pluginNamespace)
                    continue;

                list.Add(new DummyAction(ci.Key.ItemIdentifier, () =>
                {
                    if (!CustomItemManager.TryGetItem(ci.Key, out var item))
                    {
                        LastDummyCommandIssuer.Player.SendConsoleMessage($"Could not find an item under the namespace of {ci.Key}. Please make sure that item exists.");
                        return;
                    }
                    int count = 0;
                    if (item.TryGiveItem(Player.Get(referenceHub))) count++;
                    LastDummyCommandIssuer.Player.SendConsoleMessage($"Item ({ci.Key}) has been given to a player!");
                }));
            }
            return list;
        }, (sender) => sender.CheckPermission(PlayerPermissions.GivingItems));
    }

    public override List<DummyAction> AddAction(ReferenceHub hub)
    {
        List<DummyAction> list = [];
        var foundNamespaces = HashSetPool<string>.Shared.Rent();
        foreach (var item in CustomItemManager.items)
        {
            if (foundNamespaces.Contains(item.Key.PluginNamespace))
                continue;

            foundNamespaces.Add(item.Key.PluginNamespace);

            list.Add(
                new DummyAction($"{item.Key.PluginNamespace}", () =>
                {
                    RegisterNameSpace(item.Key.PluginNamespace);
                })
            );
        }
        return list;
    }

    public override bool MayExecuteThis(ICommandSender sender)
    {
        return sender.CheckPermission(PlayerPermissions.GivingItems);
    }
}