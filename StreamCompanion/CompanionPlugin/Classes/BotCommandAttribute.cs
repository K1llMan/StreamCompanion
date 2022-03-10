namespace CompanionPlugin.Classes;

public class BotCommandAttribute : Attribute
{
    public string[] Aliases { get; }

    public BotCommandAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}