using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Services;

public class StreamEventsService : IEventBus
{
    #region Поля

    private Dictionary<string, List<EventBusHandler>> subscriptions = new ();

    #endregion Поля

    #region Основные функции

    public void Publish(StreamEvent streamEvent)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(string eventName, EventBusHandler handler)
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe(string eventName, EventBusHandler handler)
    {
        throw new NotImplementedException();
    }

    #endregion Основные функции
}