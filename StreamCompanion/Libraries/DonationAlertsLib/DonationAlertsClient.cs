using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Web;

using DonationAlertsLib.Models;
using DonationAlertsLib.Models.Sockets;
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



    #region Вспомогательные функции

    private string GetClientUUID()
    {
        if (!socketClient.Connected)
            return string.Empty;

        string socketToken = apiClient.GetOAuth().SocketConnectionToken;

        SocketResponse<ClientUUIDResponse> response = socketClient.SendMessage<TokenRequest, ClientUUIDResponse>(
            new SocketRequest<TokenRequest> {
                Id = 1,
                Params = new TokenRequest {
                    Token = socketToken
                }
            });

        return response.Result.Client;
    }

    #endregion Вспомогательные функции

    #region Основные функции



    public bool Connect()
    {
        apiClient = new DAApiClient(token);
        socketClient = new DASocketClient(wsUri);

        return socketClient.Connect();
    }

    public DonationAlertsClient(string userToken)
    {
        token = userToken;
    }

    #endregion Основные функции
}