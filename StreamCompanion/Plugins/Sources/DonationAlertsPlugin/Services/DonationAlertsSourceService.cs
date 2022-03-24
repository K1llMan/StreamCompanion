using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Interfaces;

using DonationAlertsLib;
using DonationAlertsLib.Web;

using DonationAlertsPlugin.Classes;

namespace DonationAlertsPlugin.Services;

[StreamService("Donation Alerts", "donationAlerts.json")]
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
        SetConfig(serviceConfig);
    }

    public override void Init()
    {
        if (config.Value.IsProperlyConfigured())
            return;

        client = new DonationAlertsClient(config.Value.ApiToken);
        client.Connect();
        client.AddReceiveHandler(ReceiveDonationHandler);
        client.BeginReceive();
    }

    #endregion Основные функции

    #region IDisposable

    public override void Dispose()
    {
        client?.Disconnect();
    }

    #endregion IDisposable
}