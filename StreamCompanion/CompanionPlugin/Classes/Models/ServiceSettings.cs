using System.Text.Json;
using System.Text.Json.Serialization;

using CompanionPlugin.Interfaces;

using Json.Schema;
using Json.Schema.Generation;

namespace CompanionPlugin.Classes.Models;

public class ServiceSettings : IServiceSettings
{
    #region Свойства

    [Title("Доступность сервиса")]
    public bool Enabled { get; set; }
    [JsonExclude]
    [JsonIgnore]
    public JsonSchema Schema { get; set; }

    #endregion Свойства

    #region Основные функции

    /// <summary>
    /// Проверка заполненности необходимых полей
    /// </summary>
    public virtual bool IsProperlyConfigured()
    {
        return Enabled;
    }

    /// <summary>
    /// Валидация json с данными конфигурации
    /// </summary>
    /// <param name="data">Данные</param>
    public bool Validate(string data)
    {
        JsonDocument jsonData = JsonDocument.Parse(data);

        ValidationResults result = Schema.Validate(jsonData.RootElement);

        return result.IsValid;
    }

    public ServiceSettings()
    {
        SchemaGeneratorConfiguration config = new() {
            PropertyNamingMethod = PropertyNamingMethods.CamelCase
        };

        Schema = new JsonSchemaBuilder()
            .FromType(GetType(), config)
            .Build();
    }

    #endregion Основные функции
}