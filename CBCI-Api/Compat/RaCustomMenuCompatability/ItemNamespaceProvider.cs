using CustomItemLib.API;
using CustomItemLib.Patches;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;

namespace CustomItemLib.Compat.RaCustomMenuCompatability;

/// <summary>
/// Provides the RaCustomMenu functionality allowing easier giving of items.
/// </summary>
public class ItemNamespaceProvider : RaCustomMenu.API.Provider
{
    public override string CategoryName => "CBCI Give Item";
    public override bool IsDirty { get; } = true;

    private static readonly List<string> NamespacesRegistered = [];

    private static void RegisterNameSpace(string pluginNamespace)
    {
        RaCustomMenu.API.Provider.RegisterDynamicProvider($"{pluginNamespace}", true, referenceHub =>
        {
            List<RaCustomMenu.API.LimitedDummyAction> list = [new("<color=red>[CLOSE]</color>", (sender) =>
            {
                if (!sender.CheckPermission(PlayerPermissions.GivingItems)) return;
                RaCustomMenu.API.Provider.UnregisterDynamicProvider($"{pluginNamespace}");
            })];
            foreach (var ci in CustomItemManager.items)
            {
                if (ci.Key.PluginNamespace != pluginNamespace)
                    continue;

                list.Add(new(ci.Key.ItemIdentifier, (sender) =>
                {
                    if (!sender.CheckPermission(PlayerPermissions.GivingItems)) return;
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
        });
    }

    public override List<RaCustomMenu.API.LimitedDummyAction> AddActions(ReferenceHub hub)
    {
        List<RaCustomMenu.API.LimitedDummyAction> list = [];
        var foundNamespaces = HashSetPool<string>.Shared.Rent();
        foreach (var item in CustomItemManager.items)
        {
            if (foundNamespaces.Contains(item.Key.PluginNamespace))
                continue;

            foundNamespaces.Add(item.Key.PluginNamespace);

            list.Add(
                new($"{item.Key.PluginNamespace}", (sender) =>
                {
                    if (!sender.CheckPermission(PlayerPermissions.GivingItems)) return;
                    RegisterNameSpace(item.Key.PluginNamespace);
                })
            );
        }
        return list;
        // sender.CheckPermission(PlayerPermissions.GivingItems)
    }
}