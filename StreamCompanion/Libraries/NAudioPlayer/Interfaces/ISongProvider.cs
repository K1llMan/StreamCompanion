using NAudioPlayer.Classes;

namespace NAudioPlayer.Interfaces;

public interface ISongProvider
{
    bool IsCorrectUrl(string url);
    SongInfo GetSong(string cachePath, string url);
}