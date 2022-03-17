using NAudioPlayer.Classes.Providers.Local;
using NAudioPlayer.Classes.Providers.Youtube;

namespace AudioPlayerPlugin.Classes;

public class ProvidersConfig
{
    public YoutubeSongProviderConfig Youtube { get; set; }
    public LocalSongProviderConfig Local { get; set; }
}