using CompanionPlugin.Interfaces;

namespace DonationAlertsPlugin.Classes;

public class DonationAlertsConfig : IServiceSettings
{
    public bool Enabled { get; set; }
    public string ApiToken { get; set; }
}