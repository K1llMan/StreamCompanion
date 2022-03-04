using CompanionPlugin.Classes;

namespace CompanionPlugin.Interfaces;

/// <summary>
/// Обработчик событий на шине
/// </summary>
/// <param name="streamEvent">Событие</param>
public delegate void EventBusHandler(StreamEvent streamEvent);

/// <summary>
/// Интерфейс шины событий
/// </summary>
public interface IEventBus
{
    void Publish(StreamEvent streamEvent);
    void Subscribe(string eventName, EventBusHandler handler);
    void Unsubscribe(string eventName, EventBusHandler handler);
}