using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

namespace CryingHomunculusPlugin.Classes;

public class HomunculusServiceConfig : ICommandServiceSettings
{
    public bool Enabled { get; set; }
    public int Timeout { get; set; }
    public string AudioPath { get; set; }
    public float Volume { get; set; }
    public SpeechSettings Speech { get; set; } = new();
    public List<VoiceCommand> Commands { get; set; } = new();
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}