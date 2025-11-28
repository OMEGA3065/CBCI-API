using System;
using CustomItemLib.API;
using CustomItemLib.API.DefaultComponents;
using InventorySystem.Items.Pickups;
using LabApi.Features.Console;
using MEC;
using ProjectMER.Features;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItemLib.API.DefaultComponents.ProjectMER;

public interface IItemPickupSchematic
{
    public string PickupSchematicName { get; }
}

// public interface IItemPickupSizedSchematic
// {
//     public Vector3 PickupSchematicSize { get; }
// }

public class PickupModel<T> : ComponentBase<T>
    where T : ItemInstanceBase, IItemPickupSchematic
{
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        ItemPickupBase.OnPickupAdded += (ev) => OnPickupAdded(ev, itemInstance);
    }

    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        ItemPickupBase.OnPickupAdded -= (ev) => OnPickupAdded(ev, itemInstance);
    }

    private void OnPickupAdded(ItemPickupBase ev, T itemInstance)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (!itemInstance.Check(ev)) return;
            if (ObjectSpawner.TrySpawnSchematic(itemInstance.PickupSchematicName, Vector3.zero, Quaternion.identity, new Vector3(0.35f, 0.35f, 0.35f), out var schematic))
            {
                schematic.transform.SetParent(ev.transform, false);
                return;
            }
            Logger.Warn($"Failed to load schematic. Possibly a missing schematic ({itemInstance.PickupSchematicName}). No Item model loaded.");
        });
    }
}
