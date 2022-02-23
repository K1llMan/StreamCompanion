using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CompanionPlugin.Classes;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    #region Поля

    private readonly string settingsPath;
    private readonly IOptionsMonitor<T> options;
    private readonly IConfigurationRoot configuration;
    private readonly string file;

    #endregion Поля

    #region Свойства

    public string SettingsPath => settingsPath;
    public T Value => configuration.Get<T>();
    public T Get(string name) => options.Get(name);

    #endregion Свойства

    #region Вспомогательные функции

    private void InitSettings()
    {
        if (!File.Exists(settingsPath) || new FileInfo(settingsPath).Length == 0)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            File.Create(settingsPath).Close();

            File.WriteAllText(settingsPath, JsonSerializer.Serialize(new T(), ServiceJsonSerializerSettings.GetSettings()));
        }
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public WritableOptions(PluginSettings settings, IOptionsMonitor<T> options, string file)
    {
        settingsPath = Path.Combine(settings.PluginConfigPath, file);

        InitSettings();

        configuration = new ConfigurationBuilder()
            .AddJsonFile(settingsPath, true, true)
            .Build();
        
        this.options = options;
        this.file = file;
    }

    public void Update(Action<T> applyChanges)
    {
        string data = File.ReadAllText(settingsPath);

        JsonNode jsonNode = string.IsNullOrEmpty(data) 
            ? new JsonObject() 
            : JsonNode.Parse(data) ?? new JsonObject();
        T sectionObject = jsonNode.Deserialize<T>(ServiceJsonSerializerSettings.GetSettings()) ?? new T();

        applyChanges(sectionObject);

        File.WriteAllText(settingsPath, JsonSerializer.Serialize(sectionObject, ServiceJsonSerializerSettings.GetSettings()));

        configuration.Reload();
    }

    #endregion Основные функции
}