using DonationAlertsLib.Models.Common;

namespace DonationAlertsLib.Models.Sockets;

public class SubscribeResponse
{
    public int Type { get; set; }
    public string Channel { get; set; }
    public SubscribeData Data { get; set; }
}