using CompanionPlugin.Enums;

namespace CompanionPlugin.Classes;

public delegate BotMessage MessageHandler(BotMessage message);

public class CommandInfo
{
    public string Command { get; set; }
    public string Description { get; set; }
    public UserRole Role { get; set; }
    public MessageHandler Handler { get; set; }
}