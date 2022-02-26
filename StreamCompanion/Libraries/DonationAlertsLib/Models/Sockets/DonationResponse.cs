using DonationAlertsLib.Models.Common;

namespace DonationAlertsLib.Models.Sockets;

public class DonationResponse
{
    public string Channel { get; set; }
    public DonationSeqData Data { get; set; }
}