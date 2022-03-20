using CompanionPlugin.Enums;

namespace CompanionPlugin.Classes.Models;

public class BotMessage
{
    #region Свойства

    public string Command { get; set; }
    public string Text { get; set; }
    public string User { get; set; }
    public UserRole Role { get; set; }
    public MessageType Type { get; set; }

    #endregion Свойства
}