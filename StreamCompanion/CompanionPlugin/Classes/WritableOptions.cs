using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace CompanionPlugin.Classes;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    #region Поля

    private readonly string settingsPath;
    private readonly IOptionsMonitor<T> options;
    private readonly IConfigurationRoot configuration;
    private byte[] settingsFileHash;

    #endregion Поля

    #region Свойства

    public string SettingsPath => settingsPath;
    public T Value => configuration.Get<T>();
    public T Get(string name) => options.Get(name);

    #endregion Свойства

    #region Вспомогательные функции

    private byte[] ComputeHash(string filePath)
    {
        int runCount = 1;

        while (runCount < 4)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using var fs = File.OpenRead(filePath);
                    return System.Security.Cryptography.SHA1
                        .Create().ComputeHash(fs);
                }

                throw new FileNotFoundException();
            }
            catch (IOException ex)
            {
                if (runCount == 3 || ex.HResult != -2147024864)
                {
                    throw;
                }

                Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, runCount)));
                runCount++;
            }
        }

        return new byte[20];
    }

    private void InitSettings()
    {
        if (!File.Exists(settingsPath) || new FileInfo(settingsPath).Length == 0)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            File.Create(settingsPath).Close();

            File.WriteAllText(settingsPath, JsonSerializer.Serialize(new T(), ServiceJsonSerializerSettings.GetSettings()));
        }

        settingsFileHash = ComputeHash(settingsPath);
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

        Update();

        this.options = options;
    }

    /// <summary>
    /// Обновление опций
    /// </summary>
    /// <param name="applyChanges">Функция, вносящая изменения в опции</param>
    public void Update(Func<T,T> applyChanges = null)
    {
        string data = File.ReadAllText(settingsPath);

        JsonNode jsonNode = string.IsNullOrEmpty(data) 
            ? new JsonObject() 
            : JsonNode.Parse(data) ?? new JsonObject();
        T sectionObject = jsonNode.Deserialize<T>(ServiceJsonSerializerSettings.GetSettings()) ?? new T();

        if (applyChanges != null)
            sectionObject = applyChanges(sectionObject);

        File.WriteAllText(settingsPath, JsonSerializer.Serialize(sectionObject, ServiceJsonSerializerSettings.GetSettings()));

        configuration.Reload();
    }

    /// <summary>
    /// Добавление обработчика изменений
    /// </summary>
    /// <param name="handler">Обработчик</param>
    public void OnChange(Action<T> handler)
    {
        ChangeToken.OnChange(
            () => configuration.GetReloadToken(),
            c => {
                // Проверка хэша, чтобы не обновлять много раз
                byte[] hash = ComputeHash(settingsPath);
                if (!hash.SequenceEqual(settingsFileHash))
                {
                    settingsFileHash = hash;
                    handler.Invoke(c);
                }
            },
            options.CurrentValue);
    }

    #endregion Основные функции
}