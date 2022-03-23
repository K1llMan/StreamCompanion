using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

namespace DiscordPlugin.Classes;

public class DiscordSourceServiceConfig : ServiceSettings, ICommandSourceServiceSettings
{
    public string Token { get; set; }
    public bool SubscribeToEvents { get; set; }
}