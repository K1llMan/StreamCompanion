using StreamEvents.Events;

namespace StreamEvents.Interfaces;

/// <summary>
/// Обработчик событий на шине
/// </summary>
/// <param name="streamEvent">Событие</param>
public delegate Task StreamEventHandler<in T>(T streamEvent) where T : StreamEvent;

/// <summary>
/// Интерфейс шины событий
/// </summary>
public interface IStreamEventsService
{
    void Publish<T>(T streamEvent) where T : StreamEvent;
    void Subscribe<T>(StreamEventHandler<T> handler) where T : StreamEvent;
    void Unsubscribe<T>(StreamEventHandler<T> handler) where T : StreamEvent;
}