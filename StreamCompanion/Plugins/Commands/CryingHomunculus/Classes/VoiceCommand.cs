using CompanionPlugin.Enums;

namespace CryingHomunculus.Classes;

public class VoiceCommand
{
    public string Command { get; set; }
    public string Description { get; set; }
    public UserRole Role { get; set; }
    public string FilePath { get; set; }
}