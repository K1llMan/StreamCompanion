using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

namespace AudioPlayerPlugin.Classes;

public class AudioPlayerServiceConfig : ICommandServiceSettings
{
    public bool Enabled { get; set; }
    public float Volume { get; set; }
    public string CachePath { get; set; }
    public string FFMpegPath { get; set; }
    public ProvidersConfig Providers { get; set; } = new();
    public List<CommandConstraints> CommandConstraints { get; set; }
}