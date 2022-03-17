using NAudioPlayer.Interfaces;

using SimplifiedSearch;

namespace NAudioPlayer.Classes.Providers.Local;

public class LocalSongProvider : CommonProvider, ISongProvider
{
    #region Поля

    private LocalSongProviderConfig config;
    private List<string> songs = new ();

    #endregion Поля

    #region Вспомогательные функции

    private string PreparePath(string path)
    {
        return Path.GetRelativePath(config.SongsPath, path)
            .Replace("\\", " ")
            .Replace("_", " ");
    }

    #endregion Вспомогательные функции

    public bool IsCorrectUrl(string url)
    {
        return !CheckString(url, "$https?://");
    }

    public SongInfo GetSong(string cachePath, string path)
    {
        string fileName = songs.SimplifiedSearchAsync(path)
            .GetAwaiter()
            .GetResult()
            .FirstOrDefault();

        if (string.IsNullOrEmpty(fileName))
            return null;

        return new SongInfo {
            //Artist = video.Author.Title,
            Title = Path.GetFileNameWithoutExtension(fileName),
            FileName = fileName
        };
    }

    public LocalSongProvider(LocalSongProviderConfig providerConfig)
    {
        config = providerConfig;

        if (string.IsNullOrEmpty(config.SongsPath) || !Directory.Exists(config.SongsPath))
            return;

        songs = Directory
            .GetFiles(config.SongsPath, config.SearchPattern, SearchOption.AllDirectories)
            .ToList();
    }
}