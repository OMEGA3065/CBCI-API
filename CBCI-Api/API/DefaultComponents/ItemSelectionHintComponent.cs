using LabApi.Events.Arguments.PlayerEvents;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// A component used for giving the attached <see cref="CustomItemBase{T}"/>'s <see cref="LabApi.Features.Wrappers.Item"/> an on-screen hint for the player who is holding the item.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ItemSelectionHintComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase
{
    /// <inheritdoc/>
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ChangedItem += (ev) => OnOwnerChangedItem(ev, itemInstance);
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        LabApi.Events.Handlers.PlayerEvents.ChangedItem -= (ev) => OnOwnerChangedItem(ev, itemInstance);
    }

    protected void OnOwnerChangedItem(PlayerChangedItemEventArgs ev, T itemInstance)
    {
        if (ev.NewItem is null) return;
        if (ev.NewItem.Serial != itemInstance.Serial) return;
        ev.Player.SendHint($"You have selected:\n{itemInstance.Parent.Name}");
    }
}
