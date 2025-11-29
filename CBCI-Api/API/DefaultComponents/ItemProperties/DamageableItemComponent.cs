using CustomItemLib.API.DefaultComponents.ProjectMER;
using LabApi.Features.Wrappers;
using Utils.NonAllocLINQ;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// An interface defining all expected methods of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="DamageableItemComponent{T}"/>.
/// Attach either a <see cref="DamageableItemComponent{T}"/> or a <see cref="DamageableHoldableItemComponent{T}"/>
/// </summary>
public interface IDamageableItem
{
    public delegate void ApplyDamageDelegate(float damageAmount);
    public float Health { get; set; }
    public float MaxHealth { get; }

    public ApplyDamageDelegate ApplyDamage { get; set; }
}

/// <summary>
/// A component used for changing the default method of <see cref="IDamagableItem::ApplyDamage"/>.
/// In case the item is also using the <see cref="HeldItemModel{T}"/>, please use <see cref="DamageableHoldableItemComponent{T}" instead./>
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class DamageableItemComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IDamageableItem
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

/// <summary>
/// A component used for changing the default method of <see cref="IDamagableItem::ApplyDamage"/>.
/// This implementation prevents bugs that arise from items that also use the <see cref="HeldItemModel{T}"/> component./>
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class DamageableHoldableItemComponent<T> : DamageableItemComponent<T>
    where T : ItemInstanceBase, IDamageableItem, IItemHeldSchematic
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
