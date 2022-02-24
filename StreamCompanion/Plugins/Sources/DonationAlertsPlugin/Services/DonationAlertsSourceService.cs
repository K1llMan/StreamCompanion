using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

using DonationAlertsPlugin.Classes;

namespace DonationAlertsPlugin.Services;

[Description("Donation Alerts")]
public class DonationAlertsSourceService : CommandSourceService<DonationAlertsConfig>
{
    #region Основные функции

    public DonationAlertsSourceService(IWritableOptions<DonationAlertsConfig> serviceConfig)
    {
        config = serviceConfig;
    }

    public override void Init()
    {

    }

    #endregion Основные функции
}