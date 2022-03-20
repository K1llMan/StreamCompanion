using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using InformerPlugin.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InformerPlugin;

public class Informer : ICompanionPlugin
{
    #region Основные функции

    public Informer()
    {
        Name = "Informer Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddStreamService<InformerService>();
    }

    #endregion ICompanionPlugin
}
