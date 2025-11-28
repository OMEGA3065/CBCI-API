using System;
using AdminToys;
using CustomItemLib.API;
using CustomItemLib.API.DefaultComponents;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;
using WackyGrenades.CustomItems.Components;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItemLib.API.DefaultComponents.ProjectMER;

public interface IItemHeldSchematic
{
    public string HeldItemSchematicName { get; }
    public SchematicObject HeldItemAttachedSchematic { get; set; }
    public Vector3 HeldItemSchematicOffset { get; }
}

public class HeldItemModel<T> : ComponentBase<T>
    where T : ItemInstanceBase, IItemHeldSchematic
{
    private static readonly LayerMask cctvLayer = LayerMask.NameToLayer("CCTV");
    private static readonly LayerMask hitboxLayer = LayerMask.NameToLayer("Hitbox");
    
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ChangingItem += (ev) => OnInternalChangingItem(ev, itemInstance);
        ItemBase.OnItemRemoved += (ev) => ItemRemovalCleanup(ev, itemInstance);
    }

    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ChangingItem -= (ev) => OnInternalChangingItem(ev, itemInstance);
        ItemBase.OnItemRemoved -= (ev) => ItemRemovalCleanup(ev, itemInstance);
    }

    // private void AddHealth(Item item, float health)
    // {
    //     bool found = Health.TryGetValue(item.Serial, out float hp);
    //     if (!found) hp = WackyGrenadesPlugin.Instance.Config.RiotShieldHP;

    //     hp += health;

    //     if (!found)
    //         Health.Add(item.Serial, hp);
    //     else
    //         Health[item.Serial] = hp;

    //     if (hp <= 0f)
    //         RemoveItem(item);
    //     else
    //         item.Owner.ShowHint(string.Format(WackyGrenadesPlugin.Instance.Translation.RiotShieldDamaged, hp, WackyGrenadesPlugin.Instance.Config.RiotShieldHP, -health));
    // }

    // private void RemoveItem(Item item)
    // {
    //     if (item == null) return;
    //     if (!Check(item)) return;

    //     item.Destroy();
    //     DestroyPrimitives(item.Serial);
    //     shieldPrimitiveItemSerials.Remove(item.Serial);
    //     item.Owner.ShowHint(WackyGrenadesPlugin.Instance.Translation.RiotShieldDestroyed);
    // }

    private void OnInternalChangingItem(PlayerChangingItemEventArgs ev, T itemInstance)
    {
        if (ev.NewItem != null && itemInstance.Check(ev.NewItem))
        {
            OnSelecting(ev, itemInstance);
            return;
        }
        if (ev.OldItem != null && itemInstance.Check(ev.OldItem))
        {
            OnDeselecting(ev, itemInstance);
            return;
        }
    }
    protected void OnSelecting(PlayerChangingItemEventArgs ev, T itemInstance)
    {
        if (!ev.Player.IsHuman) return;
        if (!ObjectSpawner.TrySpawnSchematic("RiotShield", new(0f, 0.5f, 0.6f), new Vector3(0f, 180f, 0f), out var shieldPrimitive))
        {
            Logger.Error("Failed to load schematic. Possibly a missing schematic (RiotShield).");
            return;
        }
        shieldPrimitive.transform.SetParent(ev.Player.GameObject.transform, false);
        uint? netId = null;
        shieldPrimitive.AdminToyBases.ToList().ForEach(b =>
        {
            if (b is not PrimitiveObjectToy toy) return;

            if (!netId.HasValue) netId = b.netId;

            if (toy.NetworkPrimitiveFlags.HasFlag(PrimitiveFlags.Visible))
                toy.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
            else
                toy.NetworkPrimitiveFlags = PrimitiveFlags.None;

            if (itemInstance is not IDamagableItem damagableItem) return;
            toy.PrimitiveFlags |= (PrimitiveFlags)4;

            if (b.name.StartsWith("Hitbox"))
            {
                toy._collider.gameObject.layer = hitboxLayer;
                toy._collider.gameObject.AddComponent<DamageableObject>().Init(toy, ev.Player.ReferenceHub, (num) => damagableItem.ApplyDamage(num), netId);
            }
            else
            {
                toy._collider.gameObject.layer = cctvLayer;
            }
            toy._collider.enabled = true;
        });
        itemInstance.HeldItemAttachedSchematic = shieldPrimitive;
    }

    void ItemRemovalCleanup(ItemBase item, T itemInstance)
    {
        if (item.ItemSerial != itemInstance.Serial) return;
        DestroyPrimitives(itemInstance);
    }

    public void DestroyPrimitives(T itemInstance)
    {
        if (itemInstance?.HeldItemAttachedSchematic == null) return;
        try
        {
            itemInstance.HeldItemAttachedSchematic.Destroy();
        }
        catch (NullReferenceException)
        {
            Logger.Debug($"Destroying schematic of {itemInstance.Namespace} resulted in a NullRefException. Possibly because the primitives were already destroyed when attempting to destroy the schematic.");
        }
    }
    
    protected void OnDeselecting(PlayerChangingItemEventArgs ev, T itemInstance)
    {
        DestroyPrimitives(itemInstance);
    }

    protected void OnDroppingItem(PlayerDroppingItemEventArgs ev, T itemInstance)
    {
        if (!itemInstance.Check(ev.Item)) return;
        DestroyPrimitives(itemInstance);
    }

    protected void OnOwnerDying(PlayerDyingEventArgs ev, T itemInstance)
    {
        if (!ev.Player.Items.Any(i => itemInstance.Check(i))) return;
        DestroyPrimitives(itemInstance);
    }

    protected void OnOwnerEscaping(PlayerEscapingEventArgs ev, T itemInstance)
    {
        if (!ev.Player.Items.Any(i => itemInstance.Check(i))) return;
        DestroyPrimitives(itemInstance);
    }

    protected void OnOwnerHandcuffing(PlayerCuffingEventArgs ev, T itemInstance)
    {
        if (!ev.Target.Items.Any(i => itemInstance.Check(i))) return;
        DestroyPrimitives(itemInstance);
    }
}
