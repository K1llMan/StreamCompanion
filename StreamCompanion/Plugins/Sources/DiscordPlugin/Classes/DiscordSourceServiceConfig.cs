using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace DiscordPlugin.Classes;

public class DiscordSourceServiceConfig : ServiceSettings, ICommandSourceServiceSettings
{
    [Title("Токен бота")]
    public string Token { get; set; }
    [Title("Id канала")]
    public ulong ChannelId{ get; set; }
    [Title("Подписка на события")]
    public bool SubscribeToEvents { get; set; }

    public override bool IsProperlyConfigured()
    {
        return base.IsProperlyConfigured() && 
            !string.IsNullOrEmpty(Token) &&
            ChannelId != 0;
    }
}