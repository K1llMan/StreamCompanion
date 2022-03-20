using System.Reflection;

using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CompanionPlugin.Extensions;

public static class ServiceCollectionExtensions
{
    #region Вспомогательные функции

    private static bool HasGenericType(ParameterInfo parameter, Type genericType)
    {
        return parameter.ParameterType.IsGenericType
               && parameter.ParameterType.GetGenericTypeDefinition().IsAssignableTo(genericType);
    }

    private static void ConfigureWritable(this IServiceCollection services, Type type, string file)
    {
        services.AddTransient(typeof(IWritableOptions<>).MakeGenericType(type), provider => {
            PluginSettings settings = provider.GetService<PluginSettings>();

            object options = provider.GetService(typeof(IOptionsMonitor<>).MakeGenericType(type));

            return Activator.CreateInstance(typeof(WritableOptions<>).MakeGenericType(type), settings, options, file);
        });
    }

    #endregion Вспомогательные функции

    public static Type? FindConfigType(Type serviceType)
    {
        StreamServiceAttribute? attribute = serviceType.GetCustomAttribute<StreamServiceAttribute>();
        if (attribute == null)
            throw new Exception($"{serviceType.Name} is not a stream service");

        if (!string.IsNullOrEmpty(attribute.ConfigFilename))
        {
            ConstructorInfo? constructor = serviceType.GetConstructors()
                .FirstOrDefault(c => c.GetParameters().Any(p => HasGenericType(p, typeof(IWritableOptions<>))));

            if (constructor != null)
            {
                return constructor.GetParameters()
                    .First(p => HasGenericType(p, typeof(IWritableOptions<>)))
                    .ParameterType
                    .GetGenericArguments()
                    .First();
            }
        }

        return null;
    }

    /// <summary>
    /// Добавление сервиса для стрима
    /// </summary>
    /// <typeparam name="T">Тип сервиса</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <exception cref="Exception">Указан тип, не содержащий атрибута StreamServiceAttribute</exception>
    public static void AddStreamService<T>(this IServiceCollection services) where T : class, IStreamService
    {
        Type serviceType = typeof(T);

        StreamServiceAttribute? attribute = serviceType.GetCustomAttribute<StreamServiceAttribute>();
        if (attribute == null)
            throw new Exception($"{serviceType.Name} is not a stream service");

        // Добавление конфигурации
        Type configType = FindConfigType(serviceType);
        if (configType != null)
            services.ConfigureWritable(configType, attribute.ConfigFilename);

        // Добавление сервиса
        if (serviceType.IsAssignableTo(typeof(ICommandService)))
            services.AddSingleton(typeof(ICommandService), serviceType);

        if (serviceType.IsAssignableTo(typeof(ICommandSourceService)))
            services.AddSingleton(typeof(ICommandSourceService), serviceType);
    }
}