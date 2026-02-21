using System;
using System.Reflection;
using LabApi.Events;
using LabApi.Features.Console;

namespace CustomItemLib.API.DefaultComponents;

/// <summary>
/// The base <see cref="ICustomItemComponent{T}"/> object with default method implementation compared to the interface.
/// </summary>
/// <typeparam name="T"><inheritdoc/></typeparam>
public abstract class ComponentBase<T> : ICustomItemComponent<T> where T : ItemInstanceBase
{
    protected readonly Dictionary<ushort, EventSubscriptionManager<T>> EventSubscriptions = [];

    private EventSubscriptionManager<T> GetOrCreateManager(T item)
    {
        if (EventSubscriptions.TryGetValue(item.InstanceId, out var manager))
            return manager;
        manager = new EventSubscriptionManager<T>(item);
        EventSubscriptions[item.InstanceId] = manager;
        return manager;
    }
    
    protected Action<TEvent> GetEvent<TEvent>(T instance,
        EventSubscriptionManager<T>.SubscriptionDelegate<TEvent> method, string name = null)
    {
        name ??= method.GetMethodInfo().Name;
        var manager = GetOrCreateManager(instance);
        return manager.GetOrCreate(name, method);
    }
    
    protected Action GetEvent(T instance,
        EventSubscriptionManager<T>.SubscriptionDelegate method, string name = null)
    {
        name ??= method.GetMethodInfo().Name;
        var manager = GetOrCreateManager(instance);
        return manager.GetOrCreate(name, method);
    }
    
    protected LabEventHandler<TEvent> GetLabEvent<TEvent>(T instance,
        EventSubscriptionManager<T>.SubscriptionDelegate<TEvent> method, string name = null)
        where TEvent : EventArgs
    {
        name ??= method.GetMethodInfo().Name;
        var manager = GetOrCreateManager(instance);
        return manager.GetOrCreateLab(name, method);
    }
    
    protected LabEventHandler GetLabEvent(T instance,
        EventSubscriptionManager<T>.SubscriptionDelegate method, string name = null)
    {
        name ??= method.GetMethodInfo().Name;
        var manager = GetOrCreateManager(instance);
        return manager.GetOrCreateLab(name, method);
    }
    
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
