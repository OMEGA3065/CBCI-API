using System;
using CustomItemLib.API;
using CustomItemLib.API.DefaultComponents;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;

namespace WackyGrenades.CustomItems.ItemComponents;

/// <summary>
/// A component used for making a radio item never run of battery. <see cref="CustomItemBase{T}"/>.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class InfiniteRadioBatteryComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase
{
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.UsingRadio += (ev) => OnUsingRadio(ev, itemInstance);
    }

    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.UsingRadio -= (ev) => OnUsingRadio(ev, itemInstance);
    }

    private void OnUsingRadio(PlayerUsingRadioEventArgs ev, T itemInstance)
    {
        if (!itemInstance.Check(ev.RadioItem)) return;
        ev.IsAllowed = false;
        ev.Drain = 0;
        ev.RadioItem.BatteryPercent = 100;
    }
}
