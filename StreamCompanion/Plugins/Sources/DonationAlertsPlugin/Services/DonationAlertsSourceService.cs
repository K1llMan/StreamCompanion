using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

using DonationAlertsLib;
using DonationAlertsLib.Web;

using DonationAlertsPlugin.Classes;

namespace DonationAlertsPlugin.Services;

[Description("Donation Alerts")]
public class DonationAlertsSourceService : CommandSourceService<DonationAlertsConfig>
{
    #region Поля

    private DonationAlertsClient client;

    #endregion Поля

    #region Вспомогательные функции

    private void ReceiveDonationHandler(object sender, ReceiveEventArgs args)
    {
        Console.WriteLine(args.Data);
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public DonationAlertsSourceService(IWritableOptions<DonationAlertsConfig> serviceConfig)
    {
        config = serviceConfig;
    }

    public override void Init()
    {
        if (string.IsNullOrEmpty(config.Value.ApiToken))
            return;

        client = new DonationAlertsClient(config.Value.ApiToken);
        client.Connect();
        client.AddReceiveHandler(ReceiveDonationHandler);
        client.BeginReceive();
    }

    #endregion Основные функции
}