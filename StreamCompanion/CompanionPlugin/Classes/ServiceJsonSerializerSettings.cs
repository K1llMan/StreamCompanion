using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using Json.More;
using Json.Schema;

namespace CompanionPlugin.Classes;

public class ServiceJsonSerializerSettings
{
    public static JsonSerializerOptions GetSettings()
    {
        return new JsonSerializerOptions {
            Converters = {
                // Сериализация типов в схемах
                new EnumStringConverter<SchemaValueType>(),
                new JsonStringEnumConverter()
            },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}