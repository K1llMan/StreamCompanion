using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

using DonationAlertsPlugin.Classes;
using DonationAlertsPlugin.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonationAlertsPlugin;

public class DonationAlerts : ICompanionPlugin
{
    #region Основные функции

    public DonationAlerts()
    {
        Name = "Donation Alerts Plugin";
        Version = new Version(1, 0, 0, 0);
    }

    #endregion Основные функции

    #region ICompanionPlugin

    public string Name { get; }
    public Version Version { get; }

    public void Init(IServiceCollection services, ConfigurationManager configuration)
    {
        services.ConfigureWritable<DonationAlertsConfig>("donationAlerts.json");
        services.AddSingleton<ICommandSourceService, DonationAlertsSourceService>();
    }

    #endregion ICompanionPlugin
}
