using CompanionPlugin.Interfaces;

namespace CryingHomunculus.Classes;

public class HomunculusServiceConfig : IServiceSettings
{
    public bool Enabled { get; set; }
    public int Timeout { get; set; }
    public string AudioPath { get; set; }
    public SpeechSettings Speech { get; set; } = new();
    public List<VoiceCommand> Commands { get; set; } = new();
}