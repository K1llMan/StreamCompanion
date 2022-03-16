using AudioPlayerPlugin.Classes;
using AudioPlayerPlugin.Services;

using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioPlayerPlugin;

public class StreamAudioPlayer : ICompanionPlugin
{
    #region Основные функции

    public StreamAudioPlayer()
    {
        Name = "Audio Player Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.ConfigureWritable<AudioPlayerServiceConfig>("audioPlayer.json");
        services.AddSingleton<ICommandService, AudioPlayerService>();
    }

    #endregion ICompanionPlugin
}
