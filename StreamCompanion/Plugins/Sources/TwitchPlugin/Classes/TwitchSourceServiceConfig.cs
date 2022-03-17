using CompanionPlugin.Interfaces;

namespace TwitchPlugin.Classes;

public class TwitchSourceServiceConfig : ICommandSourceServiceSettings
{
    public bool Enabled { get; set; }
    public bool SubscribeToEvents { get; set; }
    public string Login { get; set; }
    public string ClientId { get; set; }
    public string Token { get; set; }
    public string Channel { get; set; }
}