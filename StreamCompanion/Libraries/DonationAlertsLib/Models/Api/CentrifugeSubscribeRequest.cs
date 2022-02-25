namespace DonationAlertsLib.Models.Api;

public class CentrifugeSubscribeRequestData
{
    public string[] Channels { get; set; }
    public string Client { get; set; }
}