using CustomItemLib.API;
using CustomItemLib.Patches;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using NetworkManagerUtils.Dummies;
using NorthwoodLib.Pools;
using RaCustomMenuLabApi.API;

namespace CustomItemLib.Compat.RaCustomMenu;

/// <summary>
/// Provides the RaCustomMenu functionality allowing easier giving of items.
/// </summary>
public class GiveItemsDynamicProvider : Provider
{
    private List<Player> allowedPlayers = new List<Player>();
    public override string CategoryName { get; } = "CBCI Give Item";
    public override bool IsDirty { get; } = true;

    private void RegisterNameSpace(string pluginNamespace)
    {
        RegisterDynamicProvider($"{pluginNamespace}", true, referenceHub =>
        {
            List<DummyAction> list = [];
            foreach (var ci in CustomItemManager.items)
            {
                if (ci.Key.PluginNamespace != pluginNamespace)
                    continue;

                list.Add(new DummyAction(ci.Key.ItemIdentifier, () =>
                {
                    Logger.Info($"test4: {ci.Key.ItemIdentifier}");
                    if (LastDummyCommandIssuer.Player?.HasPermission(PlayerPermissions.GivingItems) != true)
                        return;
                    Logger.Warn("Jo");
                    if (!CustomItemManager.TryGetItem(ci.Key, out var item))
                    {
                        LastDummyCommandIssuer.Player.SendConsoleMessage($"Could not find an item under the namespace of {ci.Key}. Please make sure that item exists.");
                        return;
                    }
                    Logger.Error("Foe");
                    int count = 0;
                    if (item.TryGiveItem(Player.Get(referenceHub))) count++;
                    LastDummyCommandIssuer.Player.SendConsoleMessage($"Item ({ci.Key}) has been given to a player!");
                }));
            }
            return list;
        }, allowedPlayers);
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
                    if (LastDummyCommandIssuer.Player?.HasPermission(PlayerPermissions.GivingItems) != true)
                        return;
                    allowedPlayers.Add(Player.Get(hub));
                    RegisterNameSpace(item.Key.PluginNamespace);
                })
            );
        }
        return list;
    }
}