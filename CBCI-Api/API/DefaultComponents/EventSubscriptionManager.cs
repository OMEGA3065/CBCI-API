using LabApi.Events;
using LabApi.Features.Console;

namespace CustomItemLib.API.DefaultComponents;

public class EventSubscriptionManager<T>(T item)
{
    public delegate void SubscriptionDelegate<in TEvent>(TEvent ev, T itemInstance);
    public delegate void SubscriptionDelegate(T itemInstance);

    private readonly T ItemInstance = item;

    private readonly Dictionary<string, object> _subscriptions = [];
    
    public Action<TEvent> GetOrCreate<TEvent>(string type, SubscriptionDelegate<TEvent> method)
    {
        if (_subscriptions.TryGetValue(type, out var uncast)
            )
        {
            Logger.Info($"Got an action event of {typeof(TEvent).Name} {uncast.GetType().Name}");
            if (uncast is Action<TEvent> m)
                return m;
        }

        Action<TEvent> handler = (TEvent ev) =>
        {
            method(ev, ItemInstance);
        };
        _subscriptions[type] = handler;
        Logger.Info($"CREATE an event of {typeof(TEvent).Name}");
        
        return handler;
    }
    
    public Action GetOrCreate(string type, SubscriptionDelegate method)
    {
        if (_subscriptions.TryGetValue(type, out var uncast))
        {
            Logger.Info($"Got an action event of null {uncast.GetType().Name}");
            if (uncast is Action m)
                return m;
        }

        var handler = () =>
        {
            method(ItemInstance);
        };
        _subscriptions[type] = handler;
        Logger.Info($"CREATE an event of null");
        
        return handler;
    }
    
    public LabEventHandler<TEvent> GetOrCreateLab<TEvent>(string type, SubscriptionDelegate<TEvent> method)
        where TEvent : EventArgs
    {
        if (_subscriptions.TryGetValue(type, out var uncast))
        {
            Logger.Info($"Got a labHandler event of {typeof(TEvent).Name} {uncast.GetType().Name}");
            if (uncast is LabEventHandler<TEvent> m)
                return m;
        }

        LabEventHandler<TEvent> handler = (TEvent ev) =>
        {
            method(ev, ItemInstance);
        };
        _subscriptions[type] = handler;
        Logger.Info($"CREATE a labHandler event of {typeof(TEvent).Name}");
        
        return handler;
    }
    
    public LabEventHandler GetOrCreateLab(string type, SubscriptionDelegate method)
    {
        if (_subscriptions.TryGetValue(type, out var uncast))
        {
            Logger.Info($"Got a labHandler event of null {uncast.GetType().Name}");
            if (uncast is LabEventHandler m)
                return m;
        }

        LabEventHandler handler = () =>
        {
            method(ItemInstance);
        };
        _subscriptions[type] = handler;
        Logger.Info($"CREATE a labHandler event of null");
        
        return handler;
    }
}