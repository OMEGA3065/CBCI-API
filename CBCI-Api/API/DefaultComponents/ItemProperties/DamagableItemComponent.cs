using CustomItemLib.API.DefaultComponents.ProjectMER;
using LabApi.Features.Wrappers;
using Utils.NonAllocLINQ;

namespace CustomItemLib.API.DefaultComponents;

public interface IDamagableItem
{
    public delegate void ApplyDamageDelegate(float damageAmount);
    public float Health { get; set; }
    public float MaxHealth { get; }

    public ApplyDamageDelegate ApplyDamage { get; set; }
}

public class DamagableItemComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IDamagableItem
{
    public override void OnCreatedInstance(T itemInstance)
    {
        base.OnCreatedInstance(itemInstance);
        itemInstance.ApplyDamage = (damage) => ApplyDamage(damage, itemInstance);
    }

    protected virtual void ApplyDamage(float damage, T itemInstance)
    {
        itemInstance.Health -= damage;

        var item = Item.Get(itemInstance.Serial);
        if (item == null || item.CurrentOwner == null) return;
        if (itemInstance.Health <= 0f)
            RemoveItem(item, itemInstance);
        else
            item.CurrentOwner.SendHint($"Your {itemInstance.Parent.Name} has been damaged! {itemInstance.Health}/{itemInstance.MaxHealth}");
    }

    protected virtual void RemoveItem(Item item, T itemInstance)
    {
        item.CurrentOwner.RemoveItem(item);
        item.CurrentOwner.SendHint($"Your {itemInstance.Parent.Name} has been destroyed!");
    }
}

public class DamagableHoldableItemComponent<T> : DamagableItemComponent<T>
    where T : ItemInstanceBase, IDamagableItem, IItemHeldSchematic
{
    protected override void RemoveItem(Item item, T itemInstance)
    {
        base.RemoveItem(item, itemInstance);
        if (itemInstance.Parent is not CustomItemBase<T> based) return;
        if (!based.ComponentAttributes.TryGetFirst(a => a is HeldItemModel<T>, out var component)) return;
        var heldItemModel = component as HeldItemModel<T>;
        heldItemModel.DestroyPrimitives(itemInstance);
    }
}
