using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

namespace AudioPlayerPlugin.Classes;

public class AudioPlayerServiceConfig : ServiceSettings, ICommandServiceSettings
{
    public float Volume { get; set; }
    public string CachePath { get; set; }
    public string FFMpegPath { get; set; }
    public ProvidersConfig Providers { get; set; } = new();
    public List<CommandConstraints> CommandConstraints { get; set; }
}