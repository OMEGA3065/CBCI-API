using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace CustomItemLib.API.DefaultComponents.Firearms;

/// <summary>
/// An interface defining all expected fields of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="FirearmDamageMultiplierComponent{T}"/>.
/// </summary>
public interface IFirearmDamageMultiplier
{
    public float FirearmDamageMultiplier { get; }
}

/// <summary>
/// A component used for adding a damage multiplier to the attached <see cref="CustomItemBase{T}"/>.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class FirearmDamageMultiplierComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IFirearmDamageMultiplier
{
    /// <inheritdoc/>
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        PlayerEvents.Hurting += GetLabEvent<PlayerHurtingEventArgs>(itemInstance, OnPlayerHurting, "hurting");
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        PlayerEvents.Hurting -= GetLabEvent<PlayerHurtingEventArgs>(itemInstance, OnPlayerHurting, "hurting");
    }

    protected virtual void OnPlayerHurting(PlayerHurtingEventArgs ev, T itemInstance)
    {
        if (ev.DamageHandler is not FirearmDamageHandler damageHandler) return;
        if (!itemInstance.Check(damageHandler.Firearm)) return;
        damageHandler.Damage *= itemInstance.FirearmDamageMultiplier;
    }
}