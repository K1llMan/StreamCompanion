using CompanionPlugin.Enums;

namespace CompanionPlugin.Classes.Models;

public class BotResponseMessage
{
    #region Свойства

    public string Text { get; set; }
    public MessageType Type { get; set; }

    #endregion Свойства
}