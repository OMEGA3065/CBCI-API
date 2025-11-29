using System;
using LabApi.Events.Arguments.ServerEvents;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// An interface defining all expected methods of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="ExplosionActionComponent{T}"/>.
/// </summary>
public interface IItemExplosionAction
{
    public void Explode(ProjectileExplodingEventArgs ev);
}

/// <summary>
/// A component used for adding custom OnExplosion implementation to a <see cref="CustomItemBase{T}"/>.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ExplosionActionComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IItemExplosionAction
{
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        LabApi.Events.Handlers.ServerEvents.ProjectileExploding += (ev) => OnExploding(ev, itemInstance);
    }

    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        LabApi.Events.Handlers.ServerEvents.ProjectileExploding -= (ev) => OnExploding(ev, itemInstance);
    }

    private void OnExploding(ProjectileExplodingEventArgs ev, T itemInstance)
    {
        if (!itemInstance.Check(ev.TimedGrenade)) return;
        itemInstance.Explode(ev);
    }
}
