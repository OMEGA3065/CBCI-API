using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace CustomItemLib.API.DefaultComponents.Armor;

public interface ILifecycle
{
    public void Added(ItemBase item);
    public void Removed(ItemBase item);
}

public class PickUpLifecycleComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, ILifecycle
{
    public override void SubscribeEvents(T itemInstance)
    {
        ItemBase.OnItemAdded += GetEvent<ItemBase>(itemInstance, OnItemAdded, "OnItemAdded");
        ItemBase.OnItemRemoved += GetEvent<ItemBase>(itemInstance, OnItemRemoved, "OnItemRemoved");
        PlayerEvents.Escaping += GetLabEvent<PlayerEscapingEventArgs>(itemInstance, OnOwnerEscaping, "OnOwnerEscaping");
    }

    public override void UnsubscribeEvents(T itemInstance)
    {
        ItemBase.OnItemAdded -= GetEvent<ItemBase>(itemInstance, OnItemAdded, "OnItemAdded");
        ItemBase.OnItemRemoved -= GetEvent<ItemBase>(itemInstance, OnItemRemoved, "OnItemRemoved");
        PlayerEvents.Escaping -= GetLabEvent<PlayerEscapingEventArgs>(itemInstance, OnOwnerEscaping, "OnOwnerEscaping");
    }

    private void OnItemAdded(ItemBase itemBase, T itemInstance)
    {
        if (!itemInstance.Check(itemBase)) return;
        itemInstance.Added(itemBase);
    }

    private void OnItemRemoved(ItemBase itemBase, T itemInstance)
    {
        if (!itemInstance.Check(itemBase)) return;
        itemInstance.Removed(itemBase);
    }

    private void OnOwnerEscaping(PlayerEscapingEventArgs ev, T itemInstance)
    {
        foreach (var item in ev.Player.Items)
        {
            if (!itemInstance.Check(item)) return;
            itemInstance.Removed(item.Base);
        }
    }
}