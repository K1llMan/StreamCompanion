using System;
using System.Threading;

using DonationAlertsLib;

using NAudioPlayer;
using NAudioPlayer.Classes;
using NAudioPlayer.Classes.Providers;

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
                    o.CachePath = "e:\\CProgs\\StreamCompanion\\output\\debug\\pluginConfigs\\audio\\";
                    o.FFMpegPath = "e:\\CProgs\\StreamCompanion\\output\\debug\\external\\";
                })
                .AddYoutube()
                .Build();

            player.AddFromProvider("https://www.youtube.com/watch?v=d_E7oamupsQ");
            player.AddFromProvider("test");

            player.Add(new SongInfo {
                Artist = "Test",
                Title = "Test",
                FileName = "e:\\CProgs\\StreamCompanion\\output\\debug\\pluginConfigs\\audio\\April Rain - My Silent Angel.mp3"
            });

            player.Add(new SongInfo {
                Artist = "Test",
                Title = "Test",
                FileName = "e:\\CProgs\\StreamCompanion\\output\\debug\\pluginConfigs\\audio\\Hatsune Miku feat. Kz (Livetune) – Tell Your World (Acoustic Cover, rhytm).mp3"
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