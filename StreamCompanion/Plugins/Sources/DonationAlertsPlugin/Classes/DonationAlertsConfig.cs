using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

namespace DonationAlertsPlugin.Classes;

public class DonationAlertsConfig : ServiceSettings, ICommandSourceServiceSettings
{
    public string ApiToken { get; set; }
    public bool SubscribeToEvents { get; set; }
}