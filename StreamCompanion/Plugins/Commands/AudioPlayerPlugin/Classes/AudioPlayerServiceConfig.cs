using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace AudioPlayerPlugin.Classes;

public class AudioPlayerServiceConfig : ServiceSettings, ICommandServiceSettings
{
    [Title("Громкость")]
    public float Volume { get; set; }
    [Title("Путь для кэша")]
    public string CachePath { get; set; }
    [Title("Путь к ffmpeg")]
    public string FFMpegPath { get; set; }
    [Title("Провайдеры для музыки")]
    public ProvidersConfig Providers { get; set; } = new();
    [Title("Ограничение для команд")]
    public List<CommandConstraints> CommandConstraints { get; set; }
}