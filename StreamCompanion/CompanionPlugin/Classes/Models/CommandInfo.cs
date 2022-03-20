namespace CompanionPlugin.Classes.Models;

public delegate BotResponseMessage MessageHandler(BotMessage message);

public class CommandInfo
{
    public string Command { get; set; }
    public string Description { get; set; }
    public MessageHandler Handler { get; set; }
}