using DonationAlertsLib.Models.Common;

namespace DonationAlertsLib.Models.Api;

public class CentrifugeSubscribeResponse
{
    public ChannelInfo[] Channels { get; set; }
}