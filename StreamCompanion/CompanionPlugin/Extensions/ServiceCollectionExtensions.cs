using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CompanionPlugin.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureWritable<T>(this IServiceCollection services, string file) where T : class, IServiceSettings, new()
    {
        services.AddTransient<IWritableOptions<T>>(provider => {
            PluginSettings settings = provider.GetService<PluginSettings>();

            IOptionsMonitor<T>? options = provider.GetService<IOptionsMonitor<T>>();

            return new WritableOptions<T>(settings, options, file);
        });
    }
}