using System;
using System.Threading;

using DonationAlertsLib;

using NAudioPlayer;
using NAudioPlayer.Classes;
using NAudioPlayer.Classes.Providers;
using NAudioPlayer.Classes.Providers.Local;

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
                .AddLocal(new LocalSongProviderConfig {
                    SongsPath = "E:\\Music\\",
                    SearchPattern = "*.mp3"
                })
                .Build();

            //player.AddFromProvider("https://www.youtube.com/watch?v=d_E7oamupsQ");
            player.AddFromProvider("Nightwish Siren");
            player.AddFromProvider("Ели мясо мужики");

            /*
            player.Add(new SongInfo {
                Artist = "Test",
                Title = "Test",
                FileName = "April Rain - My Silent Angel.mp3"
            });
            */

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