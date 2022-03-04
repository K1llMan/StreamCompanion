using StreamEvents.Enum;

namespace StreamEvents.Events;

public class StreamEvent
{
    public StreamEventType Type { get; protected set; }
}