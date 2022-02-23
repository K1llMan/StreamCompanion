using CompanionPlugin.Interfaces;

namespace CryingHomunculus.Classes;

public class HomunculusServiceConfig : IServiceSettings
{
    public bool Enabled { get; set; }
    public string AudioPath { get; set; }
    public List<VoiceCommand> Commands { get; set; }
}