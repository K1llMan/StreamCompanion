using System.Text.Json;
using System.Text.Json.Serialization;

namespace DonationAlertsLib.Web;

public class JsonSerializerSettings
{
    public static JsonSerializerOptions GetSettings()
    {
        return new JsonSerializerOptions {
            Converters = {
                new JsonStringEnumConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}