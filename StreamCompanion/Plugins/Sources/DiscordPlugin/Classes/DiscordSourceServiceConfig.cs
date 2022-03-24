using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace DiscordPlugin.Classes;

public class DiscordSourceServiceConfig : ServiceSettings, ICommandSourceServiceSettings
{
    [Title("Токен бота")]
    public string Token { get; set; }
    [Title("Имя сервера")]
    public string GuildName { get; set; }
    [Title("Имя канала")]
    public string ChannelName { get; set; }
    [Title("Подписка на события")]
    public bool SubscribeToEvents { get; set; }

    public override bool IsProperlyConfigured()
    {
        return base.IsProperlyConfigured() && 
            !string.IsNullOrEmpty(Token) &&
            !string.IsNullOrEmpty(GuildName) &&
            !string.IsNullOrEmpty(ChannelName);
    }
}