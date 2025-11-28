using System;
using LabApi.Events.Arguments.ServerEvents;

namespace CustomItemLib.API.DefaultComponents;

public interface IItemExplosionAction
{
    public void Explode(ProjectileExplodingEventArgs ev);
}

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
