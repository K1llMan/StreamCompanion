using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TestPlugin.Classes;
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
        services.ConfigureWritable<TestServiceConfig>("test.json");
        services.ConfigureWritable<TestSourceServiceConfig>("testSource.json");

        services.AddSingleton<ICommandService, TestService>();
        services.AddSingleton<ICommandSourceService, TestSourceService>();
    }

    #endregion ICompanionPlugin
}
