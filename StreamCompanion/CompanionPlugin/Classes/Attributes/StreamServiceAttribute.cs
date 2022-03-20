namespace CompanionPlugin.Classes.Attributes;

public class StreamServiceAttribute : Attribute
{
    public string Description { get; set; }
    public string ConfigFilename { get; set; }

    public StreamServiceAttribute(string description, string configFilename)
    {
        Description = description;
        ConfigFilename = configFilename;
    }
}