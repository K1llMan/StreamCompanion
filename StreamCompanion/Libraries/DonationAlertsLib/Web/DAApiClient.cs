using DonationAlertsLib.Models.Api;
using DonationAlertsLib.Web.Requests;

namespace DonationAlertsLib.Web;

public class DAApiClient
{
    #region Поля

    private string token;

    #endregion Поля

    #region Вспомогательные функции

    public OAuthResponse GetOAuth()
    {
        return new OAuthRequest().Get(token);
    }

    public CentrifugeSubscribeResponse Subscribe(CentrifugeSubscribeRequestData data)
    {
        return new CentrifugeSubscribeRequest().Get(token, data);
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public DAApiClient(string userToken)
    {
        token = userToken;
    }

    #endregion Основные функции
}