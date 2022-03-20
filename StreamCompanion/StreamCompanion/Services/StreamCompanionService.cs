using System.Reflection;
using System.Text.Json;

using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Enums;
using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc.ApplicationParts;

using StreamCompanion.Classes;

using StreamEvents;
using StreamEvents.Interfaces;

namespace StreamCompanion.Services;

public class StreamCompanionService
{
    #region Поля

    private readonly string settingsFilename = "settings.json";
    private ILogger<StreamCompanionService> log;

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

    private void InitLogger(IServiceCollection services)
    {
        ILoggerFactory factory = services.BuildServiceProvider().GetService<ILoggerFactory>();
        log = factory?.CreateLogger<StreamCompanionService>();
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
        // Форматтер для типа text/plain
        services.AddControllers(o => o.InputFormatters.Insert(o.InputFormatters.Count, new TextPlainInputFormatter()));

        services.AddSingleton(sp => new ServiceResolver(sp));
        services.AddSingleton<IStreamEventsService, StreamEventsService>();
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

            if (pluginTypes.Length == 0)
                continue;

            foreach (Type pluginType in pluginTypes)
            {
                ICompanionPlugin plugin = (ICompanionPlugin) Activator.CreateInstance(pluginType);
                if (plugin == null)
                    continue;

                plugin.Init(services, configuration);
                log.LogInformation($"Плагин \"{plugin.Name} ({plugin.Version})\" загружен.");
            }

            AssemblyPart part = new(assembly);
            services.AddControllers().PartManager.ApplicationParts.Add(part);
        }

        services
            // Добавление соглашения по контроллерам с шаблонными типами
            .AddMvc(o => {
                o.Conventions.Add(new GenericControllerRouteConvention());
            })
            // Добавление контроллеров с шаблонными типами
            .ConfigureApplicationPartManager(m => {
                m.FeatureProviders.Add(new StreamControllerFeature());
            });
    }

    private BotResponseMessage ProcessMessage(CommandReceivedArgs args)
    {
        foreach (ICommandService service in CommandServices)
        {
            BotResponseMessage message = service.ProcessCommand(args.Message, args.User, args.Role);
            if (message.Type != MessageType.NotCommand)
                return message;
        }

        return new BotResponseMessage {
            Type = MessageType.NotCommand
        };
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public StreamCompanionService(IServiceCollection services, ConfigurationManager configuration)
    {
        InitLogger(services);
        LoadSettings(services, configuration);
        AddCommonServices(services);
        LoadPlugins(services, configuration);
    }

    public void Init(ServiceResolver resolver)
    {
        ServiceResolver = resolver;

        // Инициализация сервисов источников
        log.LogInformation("Инициализация сервисов источников...");

        CommandSourcesServices = ServiceResolver.Resolve<ICommandSourceService>();
        CommandSourcesServices.ForEach(s => {
            s.Init();
            s.CommandReceivedEvent += ProcessMessage;

            log.LogInformation($"Сервис \"{s.GetType().Name}\" инициализирован.");
        });

        log.LogInformation("Инициализация сервисов команд...");
        CommandServices = ServiceResolver.Resolve<ICommandService>();
        CommandServices.ForEach(s => {
            s.Init();
            log.LogInformation($"Сервис \"{s.GetType().Name}\" инициализирован.");
        });

        log.LogInformation("Инициализация сервисов завершена.");
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