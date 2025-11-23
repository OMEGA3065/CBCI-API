using System;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using LiteNetLib.Utils;
using UnityEngine;

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
    protected override Item CreateItem(Player player)
    {
        var item = base.CreateItem(player);
        if (item is not FirearmItem firearm) return null;
        if (MagazineSize.HasValue)
            firearm.StoredAmmo = MagazineSize.Value;
        if (Attachments is not null)
            firearm.AttachmentsCode = firearm.ValidateAttachmentsCode(Attachments);
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