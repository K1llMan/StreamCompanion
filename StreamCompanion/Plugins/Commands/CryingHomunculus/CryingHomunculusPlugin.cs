using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using CryingHomunculus.Classes;
using CryingHomunculus.Service;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryingHomunculus;

public class CryingHomunculusPlugin : ICompanionPlugin
{
    #region Основные функции

    public CryingHomunculusPlugin()
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
