namespace NAudioPlayer.Classes;

public class AudioPlayerBuilder
{
    #region Поля

    private AudioPlayerConfig config = new () {
        Volume = 1
    };

    #endregion Поля

    #region Основные функции

    public AudioPlayerBuilder Configure(AudioPlayerConfig playerConfig)
    {
        config = playerConfig;

        return this;
    }

    public AudioPlayerBuilder Configure(Action<AudioPlayerConfig> configure)
    {
        configure(config);

        return this;
    }

    public AudioPlayer Build()
    {
        return new AudioPlayer(config);
    }

    #endregion Основные функции
}