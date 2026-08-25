using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API;

/// <summary>
/// Provides helpers for spawning custom items at room-relative locations.
/// </summary>
public static class CustomItemSpawner
{
    private const float SafeSpawnDistance = 0.2f;
    private const float SafeSpawnDistanceSquared = SafeSpawnDistance * SafeSpawnDistance;

    /// <summary>
    /// Spawns a custom item at a room-relative location.
    /// </summary>
    /// <param name="location">The room-relative location at which to spawn the item.</param>
    /// <param name="customItem">The custom item definition to spawn.</param>
    /// <returns>Whether the item was spawned successfully.</returns>
    public static bool Spawn(SpawnLocation location, ICustomItem<object> customItem)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));
        if (customItem == null) throw new ArgumentNullException(nameof(customItem));

        var roomRotation = location.Room.Rotation;
        var targetPosition = location.Room.Position + roomRotation * location.Offset;
        var targetRotation = roomRotation * location.Rotation;

        return customItem.TrySpawn(targetPosition, targetRotation);
    }

    /// <summary>
    /// Spawns a custom item at a room-relative location unless an instance of the same custom item
    /// is already within 0.2 units of the target position.
    /// </summary>
    /// <param name="location">The room-relative location at which to spawn the item.</param>
    /// <param name="customItem">The custom item definition to spawn.</param>
    /// <returns><see langword="true"/> if the item was spawned; otherwise, <see langword="false"/>.</returns>
    public static bool SafeSpawn(SpawnLocation location, ICustomItem<object> customItem)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));
        if (customItem == null) throw new ArgumentNullException(nameof(customItem));

        var targetPosition = location.Room.Position + location.Room.Rotation * location.Offset;
        if (Pickup.List.Any(pickup =>
                !pickup.IsDestroyed &&
                customItem.Check(pickup) &&
                (pickup.Position - targetPosition).sqrMagnitude <= SafeSpawnDistanceSquared))
        {
            return false;
        }

        return Spawn(location, customItem);
    }
}
