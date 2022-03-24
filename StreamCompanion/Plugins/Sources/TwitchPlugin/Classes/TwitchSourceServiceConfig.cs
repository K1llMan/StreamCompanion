using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace TwitchPlugin.Classes;

public class TwitchSourceServiceConfig : ServiceSettings, ICommandSourceServiceSettings
{
    [Title("Логин")]
    public string Login { get; set; }
    [Title("Клиент")]
    public string ClientId { get; set; }
    [Title("Токен")]
    public string Token { get; set; }
    [Title("Канал")]
    public string Channel { get; set; }
    [Title("Подписка на события")]
    public bool SubscribeToEvents { get; set; }

    public override bool IsProperlyConfigured()
    {
        return base.IsProperlyConfigured() 
            && !string.IsNullOrEmpty(Token);
    }
}