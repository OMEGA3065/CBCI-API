using System;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// An interface defining all expected fields of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="ItemLightComponent{T}"/>.
/// </summary>
public interface IItemLight
{
    public Color ItemLightColor { get; }
    public float ItemLightRange { get; }
    public float ItemLightIntensity { get; }
}

/// <summary>
/// A component used for giving the attached <see cref="CustomItemBase{T}"/>'s <see cref="LabApi.Features.Wrappers.Pickup"/> light.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ItemLightComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IItemLight
{
    /// <inheritdoc/>
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        ItemPickupBase.OnPickupAdded += (pickup) => OnPickupCreated(pickup, itemInstance);
        ItemPickupBase.OnPickupDestroyed += (pickup) => OnPickupDestroyed(pickup, itemInstance);
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        ItemPickupBase.OnPickupAdded -= (pickup) => OnPickupCreated(pickup, itemInstance);
        ItemPickupBase.OnPickupDestroyed -= (pickup) => OnPickupDestroyed(pickup, itemInstance);
    }

    protected virtual void OnPickupDestroyed(ItemPickupBase pickup, T itemInstance)
    {
        // Cleanup logic shouldn't be necessary as when the parent the Light primitive is attached to is destroyed the primitive is destroyed too.
    }

    protected virtual void OnPickupCreated(ItemPickupBase pickup, T itemInstance)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (!itemInstance.Check(pickup)) return;
            var light = LightSourceToy.Create(pickup.transform, false);
            light.Type = LightType.Point;
            light.Color = itemInstance.ItemLightColor;
            light.Intensity = itemInstance.ItemLightIntensity;
            light.Range = itemInstance.ItemLightRange;
            light.Transform.localPosition = Vector3.zero;
            light.Spawn();
        });
    }
}
