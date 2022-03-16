using NAudioPlayer.Classes.Providers.Youtube;

namespace NAudioPlayer.Classes.Providers;

public static class ProviderExtensions
{
    public static AudioPlayerBuilder AddYoutube(this AudioPlayerBuilder builder, YoutubeSongProviderConfig config = null)
    {
        builder.AddProvider<YoutubeSongProvider, YoutubeSongProviderConfig>(config);

        return builder;
    }
}