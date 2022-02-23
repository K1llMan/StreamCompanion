using System.Reflection;
using System.Text.Json;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc.ApplicationParts;

using StreamCompanion.Classes;

namespace StreamCompanion.Services;

public class StreamCompanionService
{
    #region Поля

    private readonly string settingsFilename = "settings.json";

    #endregion Поля

    #region Свойства

    /// <summary>
    /// Настройки сервиса
    /// </summary>
    public StreamCompanionSettings Settings { get; set; }

    /// <summary>
    /// Сервис поиска
    /// </summary>
    public ServiceResolver ServiceResolver { get; private set; }

    /// <summary>
    /// Сервисы-источники
    /// </summary>
    public List<ICommandSourceService> CommandSourcesServices { get; private set; }

    /// <summary>
    /// Сервисы команд
    /// </summary>
    public List<ICommandService> CommandServices { get; private set; }

    #endregion Свойства

    #region Вспомогательные функции

    private string GetRelativePath(string path)
    {
        return Path.Combine(AppContext.BaseDirectory, path);
    }

    private void LoadSettings(IServiceCollection services, ConfigurationManager configuration)
    {
        if (!File.Exists(settingsFilename))
            throw new FileNotFoundException(settingsFilename);

        Settings = JsonSerializer.Deserialize<StreamCompanionSettings>(File.ReadAllText(settingsFilename), new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Settings.Plugins = new PluginSettings {
            PluginPath = GetRelativePath(Settings.Plugins.PluginPath),
            PluginConfigPath = GetRelativePath(Settings.Plugins.PluginConfigPath),
        };

        services.AddSingleton(Settings.Plugins);
    }

    private void AddCommonServices(IServiceCollection services)
    {
        services.AddSingleton(sp => new ServiceResolver(sp));
    }

    private void LoadPlugins(IServiceCollection services, ConfigurationManager configuration)
    {
        string[] pluginFiles = Directory.GetFiles(Settings.Plugins.PluginPath, "*.dll", SearchOption.AllDirectories);

        foreach (string pluginFile in pluginFiles)
        {
            Assembly assembly = Assembly.LoadFrom(pluginFile);
            Type[] pluginTypes = assembly.GetExportedTypes()
                .Where(t => t.GetInterface(nameof(ICompanionPlugin)) != null)
                .ToArray();

            foreach (Type pluginType in pluginTypes)
            {
                ICompanionPlugin plugin = (ICompanionPlugin) Activator.CreateInstance(pluginType);
                plugin?.Init(services, configuration);
            }

            AssemblyPart part = new(assembly);
            services.AddControllers().PartManager.ApplicationParts.Add(part);
        }
    }

    private BotMessage ProcessMessage(CommandReceivedArgs args)
    {
        foreach (ICommandService service in CommandServices)
        {
            BotMessage message = service.ProcessCommand(args.Message, args.User, args.Role);
            if (message.Type != MessageType.NotCommand)
                return message;
        }

        return new BotMessage {
            Type = MessageType.NotCommand
        };
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public StreamCompanionService(IServiceCollection services, ConfigurationManager configuration)
    {
        LoadSettings(services, configuration);
        AddCommonServices(services);
        LoadPlugins(services, configuration);
    }

    public void Init(ServiceResolver resolver)
    {
        ServiceResolver = resolver;

        // Инициализация сервисов источников
        CommandSourcesServices = ServiceResolver.Resolve<ICommandSourceService>();
        CommandSourcesServices.ForEach(s => {
            s.Init();
            s.CommandReceivedEvent += ProcessMessage;
        });

        // Инициализация сервисов команд
        CommandServices = ServiceResolver.Resolve<ICommandService>();
        CommandServices.ForEach(s => s.Init());
    }

    public Dictionary<string, object> GetDescription()
    {
        return new Dictionary<string, object> {
            { "sources", CommandSourcesServices.Select(s => s.GetDescription()) },
            { "commands", CommandServices.Select(s => s.GetDescription()) }
        };
    }

    #endregion Основные функции
}