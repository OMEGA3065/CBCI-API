using System;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using LabApi.Events.Arguments.PlayerEvents;
using UnityEngine;
using LabApi.Features.Wrappers;
using LiteNetLib.Utils;

namespace CustomItemLib.API.CustomWearables;

/// <summary>
/// The base for any armor.
/// The only accepted types for this class all begin with <strong>ItemType.Armor…</strong>.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public abstract class CustomArmor<T> : CustomItemBase<T> where T : ItemInstanceBase
{
    /// <inheritdoc/>
    public override ItemType Type
    {
        get
        {
            ItemType type = base.Type;
            if (!type.ToString().StartsWith("Armor"))
                throw new InvalidTypeException($"The item type ({type}) cannot be used for the type of ({nameof(CustomArmor<T>)}). Please use one of: [{Enum.GetNames(typeof(ItemType)).Where(t => t.StartsWith("Armor")).Aggregate((a, b) => $"{a}, {b}")}]");
            return type;
        }
    }

    /// <summary>
    /// Forces the armor to have a specific helmet efficiency.
    /// Use <see cref="null"/> to keep default settings for the specific armor type.
    /// </summary>
    public virtual int? HelmetEfficiency => null;

    /// <summary>
    /// Forces the armor to have a specific body efficiency.
    /// Use <see cref="null"/> to keep default settings for the specific armor type.
    /// </summary>
    public virtual int? VestEfficiency => null;

    /// <inheritdoc/>
    protected override Item CreateItem(Player player)
    {
        var item = base.CreateItem(player);
        if (item is not BodyArmorItem firearm) return null;
        return item;
    }

    /// <inheritdoc/>
    protected override Pickup CreatePickup(Vector3? position = null)
    {
        var pickup = CreateItem(Player.Host).DropItem();
        pickup.Position = position ?? Vector3.zero;
        return pickup;
    }

    /// <inheritdoc/>
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        LabApi.Events.Handlers.PlayerEvents.PickedUpArmor += OnPickedUpArmor;
        ItemBase.OnItemAdded += OnItemAdded;
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        LabApi.Events.Handlers.PlayerEvents.PickedUpArmor -= OnPickedUpArmor;
        ItemBase.OnItemAdded -= OnItemAdded;
    }

    private void OnItemAdded(ItemBase obj)
    {
        if (!Check(obj.ItemSerial) || obj is not BodyArmor armor) return;
        ModifyItem(armor);
    }

    private void OnPickedUpArmor(PlayerPickedUpArmorEventArgs ev)
    {
        if (!Check(ev.BodyArmorItem)) return;
        if (ev.BodyArmorItem?.Base is not {} armor) return;
        ModifyItem(armor);
    }

    private void ModifyItem(BodyArmor armor)
    {
        if (HelmetEfficiency.HasValue) armor.HelmetEfficacy = HelmetEfficiency.Value;
        if (VestEfficiency.HasValue) armor.VestEfficacy = VestEfficiency.Value;
    }
}