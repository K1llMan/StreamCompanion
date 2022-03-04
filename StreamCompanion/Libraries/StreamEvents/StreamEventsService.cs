using StreamEvents.Enum;
using StreamEvents.Events;
using StreamEvents.Interfaces;

namespace StreamEvents;

public class StreamEventsService : IStreamEventsService
{
    #region Поля

    private Dictionary<StreamEventType, List<object>> subscriptions = new ();

    #endregion Поля

    #region Основные функции

    public void Publish<T>(T streamEvent) where T : StreamEvent
    {
        if (subscriptions.TryGetValue(streamEvent.Type, out List<object> handlers))
            foreach (StreamEventHandler<T> handler in handlers)
                handler.Invoke(streamEvent);
    }

    public void Subscribe<T>(StreamEventHandler<T> handler) where T : StreamEvent
    {
        StreamEventType type = Activator.CreateInstance<T>().Type;

        if (!subscriptions.ContainsKey(type)) 
            subscriptions.Add(type, new List<object>());

        if (!subscriptions[type].Contains(handler))
            subscriptions[type].Add(handler);
    }

    public void Unsubscribe<T>(StreamEventHandler<T> handler) where T : StreamEvent
    {
        StreamEventType type = Activator.CreateInstance<T>().Type;

        if (!subscriptions.ContainsKey(type))
            return;

        if (!subscriptions[type].Contains(handler))
            return;

        subscriptions[type].Remove(handler);
    }

    #endregion Основные функции
}