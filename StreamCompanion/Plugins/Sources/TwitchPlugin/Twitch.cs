using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TwitchPlugin.Services;

namespace TwitchPlugin;

public class Twitch : ICompanionPlugin
{
    #region Основные функции

    public Twitch()
    {
        Name = "Twitch Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddStreamService<TwitchSourceService>();
    }

    #endregion ICompanionPlugin
}
