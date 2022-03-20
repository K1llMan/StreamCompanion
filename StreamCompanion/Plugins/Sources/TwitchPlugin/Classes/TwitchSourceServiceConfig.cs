using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

namespace TwitchPlugin.Classes;

public class TwitchSourceServiceConfig : ServiceSettings, ICommandSourceServiceSettings
{
    public bool SubscribeToEvents { get; set; }
    public string Login { get; set; }
    public string ClientId { get; set; }
    public string Token { get; set; }
    public string Channel { get; set; }
}