using StreamEvents.Enum;

namespace StreamEvents.Events;

public class TextStreamEvent : StreamEvent
{
    #region Свойства

    public string Text { get; set; }

    #endregion Свойства

    #region Основные функции

    public TextStreamEvent()
    {
        Type = StreamEventType.Text;
    }

    #endregion Основные функции
}