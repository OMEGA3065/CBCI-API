using System;
using CustomItemLib.API;
using CustomItemLib.API.DefaultComponents;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;

namespace WackyGrenades.CustomItems.ItemComponents;

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
