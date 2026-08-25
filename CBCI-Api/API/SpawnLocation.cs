using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API;

/// <summary>
/// Describes a position and rotation relative to the center of a room.
/// </summary>
public sealed class SpawnLocation
{
    /// <summary>
    /// Initializes a new room-relative spawn location.
    /// </summary>
    /// <param name="room">The room in which the item should be spawned.</param>
    /// <param name="offset">The position offset from the room's center.</param>
    /// <param name="rotation">The rotation relative to the room.</param>
    public SpawnLocation(Room room, Vector3 offset, Quaternion rotation)
    {
        Room = room ?? throw new ArgumentNullException(nameof(room));
        Offset = offset;
        Rotation = rotation;
    }

    /// <summary>
    /// Gets the room in which the item should be spawned.
    /// </summary>
    public Room Room { get; }

    /// <summary>
    /// Gets the position offset from the room's center.
    /// </summary>
    public Vector3 Offset { get; }

    /// <summary>
    /// Gets the rotation relative to the room.
    /// </summary>
    public Quaternion Rotation { get; }
}
