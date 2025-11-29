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
public static class ItemNamespaceProvider
{
    public static string CategoryName => "CBCI Give Item";
    public static bool IsDirty => true;

    private static readonly List<string> NamespacesRegistered = [];

    public static void Init()
    {
        foreach (var space in NamespacesRegistered)
        {
            Provider.UnregisterDynamicProvider(space);
        }
        Provider.RegisterDynamicProvider(CategoryName, IsDirty, DynamicFunc, null);
    }

    private static void RegisterNameSpace(string pluginNamespace)
    {
        Provider.RegisterDynamicProvider($"{pluginNamespace}", true, referenceHub =>
        {
            List<DummyAction> list = [new DummyAction("<color=red>[CLOSE]</color>", () =>
            {
                if (LastDummyCommandIssuer.Player?.HasPermission(PlayerPermissions.GivingItems) != true)
                    return;
                Provider.UnregisterDynamicProvider($"{pluginNamespace}");
            })];
            foreach (var ci in CustomItemManager.items)
            {
                if (ci.Key.PluginNamespace != pluginNamespace)
                    continue;

                list.Add(new DummyAction(ci.Key.ItemIdentifier, () =>
                {
                    if (LastDummyCommandIssuer.Player?.HasPermission(PlayerPermissions.GivingItems) != true)
                        return;
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
        }, null);
        NamespacesRegistered.Add(pluginNamespace);
    }

    private static List<DummyAction> DynamicFunc(ReferenceHub hub)
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
                    RegisterNameSpace(item.Key.PluginNamespace);
                })
            );
        }
        return list;
    }
}