using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using LiteNetLib.Utils;
using UnityEngine;

namespace CustomItemLib.API.CustomWeapons;

/// <summary>
/// The base for any throwable grenade.
/// The only accepted types for this class are <see cref="ItemType.GrenadeHE"/> and <see cref="ItemType.GrenadeFlash"/>.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public abstract class CustomGrenade<T> : CustomItemBase<T> where T : ItemInstanceBase
{
    /// <summary>
    /// Allowed types for this class.
    /// </summary>
    private readonly ItemType[] grenadeTypes = [ItemType.GrenadeHE, ItemType.GrenadeFlash];
    
    /// <inheritdoc/>
    public override ItemType Type
    {
        get
        {
            ItemType type = base.Type;
            if (!grenadeTypes.Contains(type))
                throw new InvalidTypeException($"The item type ({type}) cannot be used for the type of ({nameof(CustomGrenade<T>)}). Please use one of: [{grenadeTypes.Select(t => t.ToString()).Aggregate((a, b) => $"{a}, {b}")}]");
            return type;
        }
    }

    /// <summary>
    /// How long the grenade takes to explode from the moment it is thrown.
    /// </summary>
    /// <value>The duration in seconds.</value>
    public virtual double FuseTime => 5f;

    /// <summary>
    /// Whether or not this grenade can actually be thrown or is just an explosive dummy item.
    /// </summary>
    /// <value><see cref="bool"/> determining whether or not the item can be thrown.</value>
    public virtual bool IsThrowable => true;

    /// <inheritdoc/>
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        LabApi.Events.Handlers.PlayerEvents.ThrowingProjectile += OnOwnerThrowingProjectile;
        LabApi.Events.Handlers.PlayerEvents.ThrewProjectile += OnOwnerThrewProjectile;
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        LabApi.Events.Handlers.PlayerEvents.ThrowingProjectile -= OnOwnerThrowingProjectile;
        LabApi.Events.Handlers.PlayerEvents.ThrewProjectile -= OnOwnerThrewProjectile;
    }

    protected virtual void OnOwnerThrowingProjectile(PlayerThrowingProjectileEventArgs ev)
    {
        if (!ev.IsAllowed || !Check(ev.ThrowableItem)) return;
        if (!IsThrowable)
        {
            ev.IsAllowed = false;
            return;
        }
    }

    protected virtual void OnOwnerThrewProjectile(PlayerThrewProjectileEventArgs ev)
    {
        if (!Check(ev.ThrowableItem)) return;
        if (ev.Projectile is not TimedGrenadeProjectile timedGrenade) return;
        timedGrenade.RemainingTime = FuseTime;
    }
}
