using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CompanionPlugin.Classes;

public class ServiceJsonSerializerSettings
{
    public static JsonSerializerOptions GetSettings()
    {
        return new JsonSerializerOptions {
            Converters = {
                new JsonStringEnumConverter()
            },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}