using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// A component used for giving the attached <see cref="CustomItemLib.API.CustomWeapons.CustomGrenade{T}"/> the ability to explode on impact.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ColliderExplodeComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase
{
    /// <inheritdoc/>
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ThrewProjectile += (ev) => OnOwnerThrewProjectile(ev, itemInstance);
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ThrewProjectile -= (ev) => OnOwnerThrewProjectile(ev, itemInstance);
    }

    protected virtual void OnOwnerThrewProjectile(PlayerThrewProjectileEventArgs ev, T itemInstance)
    {
        if (ev.ThrowableItem.Serial != itemInstance.Serial) return;
        if (ev.Projectile is not TimedGrenadeProjectile timedGrenade) return;
        timedGrenade.GameObject.AddComponent<CollisionDetector>().Init(() =>
        {
            timedGrenade.FuseEnd();
            timedGrenade.Destroy();
        }, ev.Player?.GameObject);
    }
}
