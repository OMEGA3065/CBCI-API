using System;
using CustomItemLib.Helpers;
using CustomItemLib.Patches;
using Decals;
using InventorySystem.Items;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using LiteNetLib.Utils;
using MEC;
using Mirror;
using RelativePositioning;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItemLib.API.CustomWeapons;

/// <summary>
/// The base for any regular Firearms.
/// The only accepted types for this class all begin with <strong>ItemType.Gun…</strong>.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public abstract class CustomFirearm<T> : CustomItemBase<T> where T : ItemInstanceBase
{
    /// <inheritdoc/>
    public override ItemType Type
    {
        get
        {
            ItemType type = base.Type;
            if (!type.ToString().StartsWith("Gun"))
                throw new InvalidTypeException($"The item type ({type}) cannot be used for the type of ({nameof(CustomFirearm<T>)}). Please use one of: [{Enum.GetNames(typeof(ItemType)).Where(t => t.StartsWith("Gun")).Aggregate((a, b) => $"{a}, {b}")}]");
            return type;
        }
    }

    /// <summary>
    /// Changes the firearm's magazine size.
    /// Use <see cref="null"/> to skip any magazine changes.
    /// </summary>
    public virtual int? MagazineSize => null;

    /// <summary>
    /// Forces the firearm to have certain attachments.
    /// Use <see cref="null"/> to allow any attachments.
    /// </summary>
    public virtual AttachmentName[] Attachments => null;

    /// <inheritdoc/>
    protected override Item CreateItem(Player player, ushort itemSerial)
    {
        if (MagazineSize.HasValue)
        {

        }

        var item = base.CreateItem(player, itemSerial);
        if (item is not FirearmItem firearm) return null;

        if (MagazineSize.HasValue)
        {
            FirearmMaxAmmo.MaxAmmoOverrides[firearm.Serial] = MagazineSize.Value;
            Logger.Info($"Override Set for: {firearm.Serial}");
            Timing.CallDelayed(0, () =>
            {
                if (!MagazineSize.HasValue || firearm.IsDestroyed)
                    return;
                firearm.StoredAmmo = MagazineSize.Value;
            });
        }

        if (Attachments is not null)
            firearm.AttachmentsCode = firearm.ValidateAttachmentsCode(Attachments);
        return item;
    }

    /// <inheritdoc/>
    protected override Pickup CreatePickup(Vector3? position, ushort itemSerial)
    {
        var pickup = CreateItem(Player.Host, itemSerial).DropItem();
        pickup.Position = position ?? Vector3.zero;
        if (pickup is not FirearmPickup firearm) return null;
        if (MagazineSize.HasValue)
            FirearmPickupMagazineSync.SetAmmoForPickup(firearm.Type, firearm.Serial, MagazineSize.Value);
        return pickup;
    }

    /// <inheritdoc/>
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        LabApi.Events.Handlers.PlayerEvents.ReloadingWeapon += OnOwnerReloadingWeapon;
        LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon += OnOwnerReloadedWeapon;
        LabApi.Events.Handlers.PlayerEvents.ChangingAttachments += OnOwnerChangingAttachments;
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        LabApi.Events.Handlers.PlayerEvents.ReloadingWeapon -= OnOwnerReloadingWeapon;
        LabApi.Events.Handlers.PlayerEvents.ReloadedWeapon -= OnOwnerReloadedWeapon;
        LabApi.Events.Handlers.PlayerEvents.ChangingAttachments -= OnOwnerChangingAttachments;
    }

    private void OnOwnerChangingAttachments(PlayerChangingAttachmentsEventArgs ev)
    {
        if (Attachments is null) return;
        if (!Check(ev.FirearmItem)) return;
        ev.IsAllowed = false;
    }

    protected virtual void OnOwnerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
    {
        if (MagazineSize is null) return;
        if (!Check(ev.FirearmItem)) return;
        ev.IsAllowed = ev.FirearmItem.StoredAmmo < MagazineSize.Value;
    }

    protected virtual void OnOwnerReloadedWeapon(PlayerReloadedWeaponEventArgs ev)
    {
        if (MagazineSize is null) return;
        if (!Check(ev.FirearmItem)) return;
        int targetAmmo = MagazineSize.Value;

        ItemType ammoType = ev.FirearmItem.AmmoType;
        int firearmAmmo = ev.FirearmItem.StoredAmmo;
        int playerAmmo = ev.Player.GetAmmo(ammoType);

        if (targetAmmo < firearmAmmo)
        {
            ev.FirearmItem.StoredAmmo = targetAmmo;
            ev.Player.SetAmmo(ammoType, (ushort)(playerAmmo + (firearmAmmo - targetAmmo)));
        }
        else if (targetAmmo > firearmAmmo)
        {
            int ammoLeftToAdd = targetAmmo - firearmAmmo;
            if (ammoLeftToAdd > playerAmmo)
                ammoLeftToAdd = playerAmmo;
            ev.FirearmItem.StoredAmmo = firearmAmmo + ammoLeftToAdd;
            ev.Player.SetAmmo(ammoType, (ushort)(playerAmmo - ammoLeftToAdd));
        }
    }
}