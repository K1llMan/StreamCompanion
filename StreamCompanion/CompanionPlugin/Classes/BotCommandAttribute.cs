using CompanionPlugin.Enums;

namespace CompanionPlugin.Classes;

public class BotCommandAttribute : Attribute
{
    public string Command { get; }

    public BotCommandAttribute(string command)
    {
        Command = command;
    }
}