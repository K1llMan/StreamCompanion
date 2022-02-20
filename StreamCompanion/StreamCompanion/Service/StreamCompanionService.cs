using System.Reflection;

using CompanionPlugin;
using CompanionPlugin.Interfaces;

using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace StreamCompanion.Service;

public class StreamCompanionService
{
    #region Поля


    #endregion Поля

    #region Свойства

    public IServiceCollection Services { get; set; }

    public string PluginDir { get; }

    #endregion Свойства

    #region Вспомогательные функции

    private void LoadPlugins()
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
                    plugin.Init(Services);
            }

            AssemblyPart part = new(assembly);
            Services.AddControllers().PartManager.ApplicationParts.Add(part);
        }

    }

    #endregion Вспомогательные функции


    #region Основные функции

    public StreamCompanionService(IServiceCollection services)
    {
        Services = services;
        PluginDir = Path.Combine(AppContext.BaseDirectory, "plugins");

        LoadPlugins();
    }

    #endregion Основные функции
}