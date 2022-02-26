using System;

using DonationAlertsLib;

using Xunit;

namespace StreamCompanionTests
{
    public class CommonTests
    {
        [Fact]
        public void SocketsClientTest()
        {
            string apiToken = "key";

            DonationAlertsClient client = new(apiToken);
            client.Connect();

            client.AddReceiveHandler((o, a) => {
                Console.WriteLine(a.Data);
            });

            client.BeginReceive();

            Console.WriteLine("Connected");
        }
    }
}