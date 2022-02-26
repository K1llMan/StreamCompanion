using DonationAlertsLib.Models.Api;
using DonationAlertsLib.Web;

namespace DonationAlertsLib;

public class DonationAlertsClient
{
    #region Поля

    private string token;
    private readonly string wsUri = "wss://centrifugo.donationalerts.com/connection/websocket";

    private DAApiClient apiClient;
    private DASocketClient socketClient;

    #endregion Поля

    #region Основные функции

    public void BeginReceive()
    {
        OAuthResponse oauth = apiClient.GetOAuth();

        string clientUuuid = socketClient.GetClientUuid(oauth.SocketConnectionToken);

        CentrifugeSubscribeResponse subs = apiClient.Subscribe(new CentrifugeSubscribeRequestData {
            Client = clientUuuid,
            Channels = new[] { $"$alerts:donation_{oauth.Id}" }
        });

        socketClient.Subscribe(subs.Channels);

        socketClient.BeginReceive();
    }

    public bool Connect()
    {
        apiClient = new DAApiClient(token);
        socketClient = new DASocketClient(wsUri);

        return socketClient.Connect();
    }

    public void Disconnect()
    {
        socketClient.Disconnect();
    }

    public void AddReceiveHandler(RecieveEventHandler handler)
    {
        socketClient.OnReceive += handler;
    }

    public DonationAlertsClient(string apiToken)
    {
        token = apiToken;
    }

    #endregion Основные функции
}