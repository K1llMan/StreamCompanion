using CompanionPlugin.Enums;

namespace CompanionPlugin.Classes;

public class BotCommandAttribute : Attribute
{
    public string Command { get; }
    public UserRole Role { get; }

    public BotCommandAttribute(string command, UserRole role)
    {
        Command = command;
        Role = role;
    }
}