using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanionPlugin.Interfaces;

public interface ICompanionPlugin
{
    string Name { get; }
    Version Version { get; }
    void Init(IServiceCollection services, ConfigurationManager configuration);
}