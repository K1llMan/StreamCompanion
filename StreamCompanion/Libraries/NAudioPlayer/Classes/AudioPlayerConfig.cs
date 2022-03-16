namespace NAudioPlayer.Classes;

public class AudioPlayerConfig
{
    #region Поля

    private float volume;

    #endregion Поля

    public float Volume
    {
        get => volume;
        set => volume = Math.Max(Math.Min(value, 1), 0);
    }

    public string CachePath { get; set; }
    public string FFMpegPath { get; set; }
}