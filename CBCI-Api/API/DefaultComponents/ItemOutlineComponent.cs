using Effects.Outlines;
using Effects.Outlines.OutlineModifiers;
using InventorySystem.Items.Pickups;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Utils;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// An interface defining all expected fields of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="ItemOutlineComponent{T}"/>.
/// </summary>
public interface IItemOutline
{
    public Color ItemOutlineColor { get; }
    public float ItemOutlineWidth { get; }
    public bool ItemOutlineFull { get; }
}

/// <summary>
/// An interface used to detect whether an item instance should have outline animations.
/// Used by <see cref="ItemOutlineComponent{T}"/>.
/// </summary>
public interface IItemAnimatedOutline;

/// <summary>
/// An interface defining all expected fields of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="ItemOutlineComponent{T}"/>.
/// </summary>
public interface IItemAnimatedOutlineColor : IItemAnimatedOutline
{
    public AnimationColorCurve ItemOutlineColorCurve { get; }
}

/// <summary>
/// An interface defining all expected fields of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="ItemOutlineComponent{T}"/>.
/// </summary>
public interface IItemAnimatedOutlineWidth : IItemAnimatedOutline
{
    public AnimationCurve ItemOutlineWidthCurve { get; }
}

/// <summary>
/// A component used for giving the attached <see cref="CustomItemBase{T}"/>'s <see cref="LabApi.Features.Wrappers.Pickup"/> an outline.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ItemOutlineComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IItemOutline
{
    private readonly Dictionary<ushort, OutlineEffect> _outlineEffects = [];

    public override void InitComponent(ICustomItem<T> item)
    {
        base.InitComponent(item);
        Reset();
        ServerEvents.RoundRestarted += Reset;
    }

    private void Reset()
    {
        foreach (var (serial, outline) in _outlineEffects)
        {
            if (!Pickup.TryGet(serial, out var pickup))
                continue;
            pickup.TryRemoveOutline(outline);
        }

        _outlineEffects.Clear();
    }

    /// <inheritdoc/>
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        ItemPickupBase.OnPickupAdded += GetEvent<ItemPickupBase>(itemInstance, OnPickupCreated, "pickupAdded");
        ItemPickupBase.OnPickupDestroyed += GetEvent<ItemPickupBase>(itemInstance, OnPickupDestroyed, "pickupDestroyed");
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        ItemPickupBase.OnPickupAdded -= GetEvent<ItemPickupBase>(itemInstance, OnPickupCreated, "pickupAdded");
        ItemPickupBase.OnPickupDestroyed -= GetEvent<ItemPickupBase>(itemInstance, OnPickupDestroyed, "pickupDestroyed");
    }

    protected virtual void OnPickupDestroyed(ItemPickupBase pickup, T itemInstance)
    {
        // Cleanup logic shouldn't be necessary maybe?
    }

    protected virtual void OnPickupCreated(ItemPickupBase pickup, T itemInstance)
    {
        if (!itemInstance.Check(pickup)) return;
        if (_outlineEffects.ContainsKey(pickup.Info.Serial)) return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (!itemInstance.Check(pickup)) return;
            List<OutlineModifier> effects = [];

            if (itemInstance is IItemAnimatedOutline)
                effects.Add(new CurveBasedOutlineModifier(
                        itemInstance is IItemAnimatedOutlineColor animatedOutlineColor
                            ? animatedOutlineColor.ItemOutlineColorCurve : null,
                        itemInstance is IItemAnimatedOutlineWidth animatedOutlineWidth
                            ? animatedOutlineWidth.ItemOutlineWidthCurve : null,
                        null, null, null
                    ));

            var outline = new OutlineEffect(1,
                itemInstance.ItemOutlineColor,
                itemInstance.ItemOutlineWidth, 0, false,
                itemInstance.ItemOutlineFull, effects);

            if (!pickup.TryAddOutlineEffect(outline))
            {
                Logger.Error("[ItemOutlineComponent] Failed to add outline effect to pickup!");
                return;
            }

            _outlineEffects[pickup.Info.Serial] = outline;
        });
    }
}
