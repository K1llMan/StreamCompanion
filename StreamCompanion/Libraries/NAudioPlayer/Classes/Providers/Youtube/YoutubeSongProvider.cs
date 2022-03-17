using NAudioPlayer.Interfaces;

using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace NAudioPlayer.Classes.Providers.Youtube;

public class YoutubeSongProvider : CommonProvider, ISongProvider
{
    #region Поля

    private YoutubeSongProviderConfig config;
    private YoutubeClient youtube = new();

    #endregion Поля

    public bool IsCorrectUrl(string url)
    {
        return CheckString(url, "https://(www\\.)youtube.com");
    }

    public SongInfo GetSong(string cachePath, string path)
    {
        try
        {
            Video video = youtube.Videos.GetAsync(path)
                .GetAwaiter()
                .GetResult();

            StreamManifest manifest = youtube.Videos.Streams.GetManifestAsync(path)
                .GetAwaiter()
                .GetResult();

            IStreamInfo stream = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            string fileName = Path.Combine(cachePath, config.CachePath, $"{GetTempFileName(video.Author.Title, video.Title)}.{stream.Container.Name}");
            CheckDirectory(fileName);

            if (!File.Exists(fileName))
            {
                youtube.Videos.Streams.DownloadAsync(stream, fileName)
                    .GetAwaiter()
                    .GetResult();
            }

            return new SongInfo
            {
                Title = video.Title,
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public YoutubeSongProvider(YoutubeSongProviderConfig providerConfig)
    {
        config = providerConfig;
    }
}