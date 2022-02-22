using System.Reflection;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace StreamCompanion.Services;

public class StreamCompanionService
{
    #region Свойства

    /// <summary>
    /// Директория плагинов
    /// </summary>
    public string PluginDir { get; }

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

    private void AddCommonServices(IServiceCollection services)
    {
        services.AddSingleton(sp => new ServiceResolver(sp));
    }

    private void LoadPlugins(IServiceCollection services)
    {
        string[] pluginFiles = Directory.GetFiles(PluginDir, "*.dll", SearchOption.AllDirectories);

        foreach (string pluginFile in pluginFiles)
        {
            Assembly assembly = Assembly.LoadFrom(pluginFile);
            Type[] pluginTypes = assembly.GetExportedTypes()
                .Where(t => t.GetInterface(nameof(ICompanionPlugin)) != null)
                .ToArray();

            foreach (Type pluginType in pluginTypes)
            {
                ICompanionPlugin plugin = (ICompanionPlugin) Activator.CreateInstance(pluginType);
                if (plugin != null)
                    plugin.Init(services);
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

    public StreamCompanionService(IServiceCollection services)
    {
        PluginDir = Path.Combine(AppContext.BaseDirectory, "plugins");

        AddCommonServices(services);
        LoadPlugins(services);
    }

    public void Init(ServiceResolver resolver)
    {
        ServiceResolver = resolver;

        // Инициализация сервисов источников
        CommandSourcesServices = ServiceResolver.Resolve<ICommandSourceService>();
        CommandSourcesServices.ForEach(s => s.CommandReceivedEvent += ProcessMessage);

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