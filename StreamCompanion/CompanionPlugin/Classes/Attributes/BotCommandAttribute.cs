namespace CompanionPlugin.Classes.Attributes;

public class BotCommandAttribute : Attribute
{
    public string[] Aliases { get; }

    public BotCommandAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}