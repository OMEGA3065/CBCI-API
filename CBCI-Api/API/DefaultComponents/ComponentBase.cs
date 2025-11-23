using System;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// The base <see cref="ICustomItemComponent{T}"/> object with default method implementation compared to the interface.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public abstract class ComponentBase<T> : ICustomItemComponent<T>
{
    /// <inheritdoc/>
    public virtual void DestroyComponent(ICustomItem<T> item) { }

    /// <inheritdoc/>
    public virtual void InitComponent(ICustomItem<T> item) { }

    /// <inheritdoc/>
    public virtual void OnCreatedInstance(T itemInstance) => SubscribeEvents(itemInstance);

    /// <inheritdoc/>
    public virtual bool OnCreatingInstance(T itemInstance) => true;

    /// <inheritdoc/>
    public virtual void OnDestroyedInstance(T itemInstance) => UnsubscribeEvents(itemInstance);

    /// <inheritdoc/>
    public virtual bool OnDestroyingInstance(T itemInstance) => true;

    /// <inheritdoc/>
    public virtual void SubscribeEvents(T itemInstance) { }

    /// <inheritdoc/>
    public virtual void UnsubscribeEvents(T itemInstance) { }
}
