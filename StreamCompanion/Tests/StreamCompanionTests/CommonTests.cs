using System;
using System.Threading;

using DonationAlertsLib;

using NAudioPlayer;
using NAudioPlayer.Classes;

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

        [Fact]
        public void AudioPlayerTest()
        {
            AudioPlayer player = new AudioPlayerBuilder()
                .Configure(o => {
                    o.Volume = 1.5f;
                })
                .Build();

            player.Add(new SongInfo {
                Artist = "Test",
                Title = "Test",
                FileName = "D:\\SVC\\StreamCompanion\\output\\debug\\pluginConfigs\\audio\\joan-osborne-one-of-us.mp3"
            });

            player.Add(new SongInfo {
                Artist = "Test",
                Title = "Test",
                FileName = "D:\\SVC\\StreamCompanion\\output\\debug\\pluginConfigs\\audio\\01 - Nightmare.mp3"
            });

            player.Play();
            Thread.Sleep(10000);

            player.Next();
            Thread.Sleep(10000);
            player.Pause();
            Thread.Sleep(5000);
            player.Play();
            player.Volume(0.2f);
            Thread.Sleep(10000);
            player.Previous();
            player.Volume(0.7f);
            Thread.Sleep(10000);
        }
    }
}