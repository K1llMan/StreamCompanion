using DonationAlertsLib.Models.Api;

namespace DonationAlertsLib.Web.Requests;

public class CentrifugeSubscribeRequest : BaseApiRequest
{
    public CentrifugeSubscribeResponse Get(string token, CentrifugeSubscribeRequestData data)
    {
        return GetResponse<CentrifugeSubscribeRequestData, CentrifugeSubscribeResponse>(
            "api/v1/centrifuge/subscribe",
            HttpMethod.Post,
            headers: GetDefaultHeaders(token),
            body: data
        );
    }
}