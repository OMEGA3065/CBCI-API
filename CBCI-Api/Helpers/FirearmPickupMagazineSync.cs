using InventorySystem.Items;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms.Modules;
using Mirror;

namespace CustomItemLib.Helpers;

public class FirearmPickupMagazineSync
{
    private static Dictionary<ItemType, byte> subcomponentIndecies = null;

    public static void SetAmmoForPickup(ItemType itemType, ushort serial, int ammo)
    {
        if (!TryGetItemData(itemType, out var syncId))
            throw new InvalidOperationException($"Couldn't find the syncId for a `InventorySystem.Items.Firearms.Modules.MagazineModule` in specified item type ({itemType})!");

        MagazineModule.SyncData[serial] = ammo;

        using (new AutosyncRpc(new ItemIdentifier(itemType, serial), out var writer))
        {
            writer.WriteByte(syncId);
            writer.WriteUShort(serial);
            writer.WriteInt(ammo);
        }
    }

    private static bool TryGetItemData(ItemType itemType, out byte index)
    {
        if (subcomponentIndecies != null)
            return subcomponentIndecies.TryGetValue(itemType, out index);

        subcomponentIndecies = [];

        foreach (var autoItem in ModularAutosyncItem.AllTemplates)
        {
            for (byte b = 0; b < autoItem.AllSubcomponents.Length; b++)
            {
                if (autoItem.AllSubcomponents[b] is not MagazineModule)
                    continue;
                subcomponentIndecies[autoItem.ItemTypeId] = b;
            }
        }

        if (subcomponentIndecies.Count == 0)
            throw new InvalidOperationException("Couldn't find the `InventorySystem.Items.Firearms.Modules.MagazineModule` in the any ModularAutosyncItem!");

        return subcomponentIndecies.TryGetValue(itemType, out index);
    }
}