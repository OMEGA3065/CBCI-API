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

/// <summary>
/// An interface defining all expected methods of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="PickupModel{T}"/>.
/// </summary>
public interface IItemPickupSchematic
{
    public string PickupSchematicName { get; }
}

/// <summary>
/// A component used for attaching a <see cref="SchematicObject"/> to a <see cref="CustomItemBase{T}"/>.
/// This Schematic is visible when the item is dropped on the ground as a <see cref="Pickup"/>.
/// The actual item model of the pickup is not modified and is not hidden.
/// The pickup item model doesn't collide with any objects.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
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
