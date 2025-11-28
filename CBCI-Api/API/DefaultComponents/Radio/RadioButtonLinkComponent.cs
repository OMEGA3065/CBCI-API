using System;
using CustomItemLib.API;
using CustomItemLib.API.DefaultComponents;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;

namespace WackyGrenades.CustomItems.ItemComponents;

public interface IItemRadioButtons
{
    public void LeftClickRadio(PlayerChangingRadioRangeEventArgs ev);
    public void RightClickRadio(PlayerTogglingRadioEventArgs ev);
}

public class RadioButtonLinkComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IItemRadioButtons
{
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ChangingRadioRange += (ev) => OnChangingRange(ev, itemInstance);
        LabApi.Events.Handlers.PlayerEvents.TogglingRadio += (ev) => OnToggling(ev, itemInstance);
    }

    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ChangingRadioRange -= (ev) => OnChangingRange(ev, itemInstance);
        LabApi.Events.Handlers.PlayerEvents.TogglingRadio -= (ev) => OnToggling(ev, itemInstance);
    }

    private void OnChangingRange(PlayerChangingRadioRangeEventArgs ev, T itemInstance)
    {
        if (!itemInstance.Check(ev.RadioItem)) return;
        itemInstance.LeftClickRadio(ev);
    }

    private void OnToggling(PlayerTogglingRadioEventArgs ev, T itemInstance)
    {
        if (!itemInstance.Check(ev.RadioItem)) return;
        itemInstance.RightClickRadio(ev);
    }
}
