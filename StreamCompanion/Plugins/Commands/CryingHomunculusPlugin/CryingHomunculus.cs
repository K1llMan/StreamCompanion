using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using CryingHomunculusPlugin.Classes;
using CryingHomunculusPlugin.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryingHomunculusPlugin;

public class CryingHomunculus : ICompanionPlugin
{
    #region Основные функции

    public CryingHomunculus()
    {
        Name = "Crying Homunculus Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.ConfigureWritable<HomunculusServiceConfig>("cryingHomunculus.json");
        services.AddSingleton<ICommandService, HomunculusService>();
    }

    #endregion ICompanionPlugin
}
