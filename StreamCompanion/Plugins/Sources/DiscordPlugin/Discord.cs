using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using DiscordPlugin.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordPlugin;

public class Discord : ICompanionPlugin
{
    #region Основные функции

    public Discord()
    {
        Name = "Discord Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddStreamService<DiscordSourceService>();
    }

    #endregion ICompanionPlugin
}