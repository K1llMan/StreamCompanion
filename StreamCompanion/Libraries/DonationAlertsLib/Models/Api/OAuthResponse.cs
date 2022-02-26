using System.Text.Json.Serialization;

namespace DonationAlertsLib.Models.Api;

public class OAuthResponse
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Email { get; set; }
    [JsonPropertyName("socket_connection_token")]
    public string SocketConnectionToken { get; set; }
}