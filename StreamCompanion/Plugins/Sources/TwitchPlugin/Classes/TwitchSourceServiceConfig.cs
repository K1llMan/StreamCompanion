using CompanionPlugin.Interfaces;

namespace TwitchPlugin.Classes;

public class TwitchSourceServiceConfig : IServiceSettings
{
    public bool Enabled { get; set; }
    public string Login { get; set; }
    public string ClientId { get; set; }
    public string Token { get; set; }
    public string Channel { get; set; }
}