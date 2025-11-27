using System;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// An <see cref="UnityEngine.MonoBehaviour"/> handling collisions.
/// Inteded to be used with <see cref="ColliderExplodeComponent{T}"/>.
/// </summary>
public class CollisionDetector : MonoBehaviour
{
    /// <summary>
    /// Method exucted once a collision is detected.
    /// </summary>
    Action CollisionAction = null;

    /// <summary>
    /// An object, collisions with which are all ignored.
    /// Inteded to be the <see cref="GameObject"/> this <see cref="UnityEngine.MonoBehaviour"/> is attached to.
    /// </summary>
    GameObject IgnoredObject = null;

    bool CollisionActive = false;

    /// <summary>
    /// Sets up this <see cref="CollisionDetector"/>.
    /// </summary>
    public void Init(Action collisionAction, GameObject ignoredObject = null)
    {
        CollisionAction = collisionAction;
        IgnoredObject = ignoredObject;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (CollisionActive)
            return;
        if (CollisionAction is null)
            return;
        if (IgnoredObject is not null && collision?.collider?.gameObject == IgnoredObject)
            return;
        if (collision.collider.gameObject.TryGetComponent<EffectGrenade>(out _))
            return;

        CollisionActive = true;
        CollisionAction();
    }
    
    public void OnCollisionExit(Collision collision)
    {
        if (!CollisionActive)
            return;

        CollisionActive = false;
    }
}
