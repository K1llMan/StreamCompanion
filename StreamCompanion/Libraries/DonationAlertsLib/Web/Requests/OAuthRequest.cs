using DonationAlertsLib.Models.Api;

namespace DonationAlertsLib.Web.Requests;

public class OAuthRequest : BaseApiRequest
{
    public OAuthResponse Get(string token)
    {
        return GetResponse<string, ApiResponse<OAuthResponse>>(
                "api/v1/user/oauth", 
                HttpMethod.Get, 
                headers: GetDefaultHeaders(token))
            .Data;
    }
}