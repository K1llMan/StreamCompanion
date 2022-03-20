using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

namespace CryingHomunculusPlugin.Classes;

public class HomunculusServiceConfig : ServiceSettings, ICommandServiceSettings
{
    public int Timeout { get; set; }
    public string AudioPath { get; set; }
    public float Volume { get; set; }
    public SpeechSettings Speech { get; set; } = new();
    public List<VoiceCommand> Commands { get; set; } = new();
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}