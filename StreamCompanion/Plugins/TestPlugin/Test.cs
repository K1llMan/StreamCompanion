using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TestPlugin.Services;

namespace TestPlugin;

public class Test : ICompanionPlugin
{
    #region Основные функции

    public Test()
    {
        Name = "Test Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddStreamService<TestService>();
        services.AddStreamService<TestSourceService>();
    }

    #endregion ICompanionPlugin
}
