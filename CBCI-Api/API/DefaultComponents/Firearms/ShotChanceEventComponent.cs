using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerStatsSystem;
using Random = UnityEngine.Random;

namespace CustomItemLib.API.DefaultComponents.Firearms;

/// <summary>
/// An interface defining all expected fields of an <see cref="ItemInstanceBase"/>.
/// Used by <see cref="ShootChanceEventComponent{T}"/>.
/// </summary>
public interface IFirearmShotEventChance
{
    public float EventChance { get; }
    public void OnShotEventChance(PlayerShotWeaponEventArgs ev);
}

/// <summary>
/// A component used for using a method when a <see cref="CustomItemBase{T}"/> is shot and a chance is hit.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public class ShootChanceEventComponent<T> : ComponentBase<T>
    where T : ItemInstanceBase, IFirearmShotEventChance
{
    /// <inheritdoc/>
    public override void SubscribeEvents(T itemInstance)
    {
        base.SubscribeEvents(itemInstance);
        PlayerEvents.ShotWeapon += GetLabEvent<PlayerShotWeaponEventArgs>(itemInstance, OnPlayerShotWeapon, "shotWeapon");
    }

    /// <inheritdoc/>
    public override void UnsubscribeEvents(T itemInstance)
    {
        base.UnsubscribeEvents(itemInstance);
        PlayerEvents.ShotWeapon -= GetLabEvent<PlayerShotWeaponEventArgs>(itemInstance, OnPlayerShotWeapon, "shotWeapon");
    }

    protected virtual void OnPlayerShotWeapon(PlayerShotWeaponEventArgs ev, T itemInstance)
    {
        if (!itemInstance.Check(ev.FirearmItem)) return;
        if (Random.Range(0f, 1f) >= itemInstance.EventChance) return;
        itemInstance.OnShotEventChance(ev);
    }
}