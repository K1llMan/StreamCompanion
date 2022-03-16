using NAudioPlayer.Interfaces;

namespace NAudioPlayer.Classes;

public class AudioPlayerBuilder
{
    #region Поля

    private List<ISongProvider> providers = new();
    private AudioPlayerConfig config = new () {
        Volume = 1,
        CachePath = "cache"
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

    public AudioPlayerBuilder AddProvider<T, CT> (CT? providerConfig = null) 
        where T : ISongProvider
        where CT : class, new()
    {
        CT pConfig = providerConfig ?? new CT();

        ISongProvider provider = (ISongProvider) Activator.CreateInstance(typeof(T), pConfig);

        providers.Add(provider);

        return this;
    }

    public AudioPlayer Build()
    {
        return new AudioPlayer(config) {
            Providers = providers
        };
    }

    #endregion Основные функции
}