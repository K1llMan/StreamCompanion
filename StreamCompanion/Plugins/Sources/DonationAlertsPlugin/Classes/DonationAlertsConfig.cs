using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace DonationAlertsPlugin.Classes;

public class DonationAlertsConfig : ServiceSettings, ICommandSourceServiceSettings
{
    [Title("Токен API")]
    public string ApiToken { get; set; }
    [Title("Подписка на события")]
    public bool SubscribeToEvents { get; set; }

    public override bool IsProperlyConfigured()
    {
        return base.IsProperlyConfigured() && !string.IsNullOrEmpty(ApiToken);
    }
}