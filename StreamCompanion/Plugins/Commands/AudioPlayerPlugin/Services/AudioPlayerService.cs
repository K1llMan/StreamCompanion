using System.ComponentModel;
using System.Diagnostics;

using AudioPlayerPlugin.Classes;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Logging;

using NAudioPlayer;
using NAudioPlayer.Classes;
using NAudioPlayer.Classes.Providers;
using NAudioPlayer.Classes.Providers.Youtube;

using StreamEvents.Events;
using StreamEvents.Interfaces;

namespace AudioPlayerPlugin.Services;

[Description("Музыкальный плеер")]
public class AudioPlayerService : CommandService<AudioPlayerServiceConfig>
{
    #region Поля

    private IStreamEventsService eventListener;
    private ILogger<AudioPlayerService> log;

    private string playerCachePath;
    private AudioPlayer player;

    #endregion Поля

    #region Команды

    [BotCommand("!djAdd")]
    [Description("Добавить песню")]
    public BotResponseMessage Add(BotMessage message)
    {
        player.AddFromProvider(message.Text);

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djPlay")]
    [Description("Запустить проигрыватель")]
    public BotResponseMessage Play(BotMessage message)
    {
        player.Play();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djPause")]
    [Description("Поставить проигрыватель на паузу")]
    public BotResponseMessage Pause(BotMessage message)
    {
        player.Pause();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djStop")]
    [Description("Остановить проигрыватель")]
    public BotResponseMessage Stop(BotMessage message)
    {
        player.Stop();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djVolume")]
    [Description("Установить уровень громкости")]
    public BotResponseMessage Volume(BotMessage message)
    {
        if (!float.TryParse(message.Command, out float volume))
            return new BotResponseMessage {
                Text = "Неверное значение громкости",
                Type = MessageType.Error
            };

        player.Volume(volume);

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djNext")]
    [Description("Следующий трек")]
    public BotResponseMessage Next(BotMessage message)
    {
        player.Next();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djPrev")]
    [Description("Предыдущий трек")]
    public BotResponseMessage Prev(BotMessage message)
    {
        player.Previous();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    #endregion Команды

    #region Вспомогательные функции

    private string GetCorrectPaths(string path)
    {
        return !Directory.Exists(path)
            ? Path.Combine(AppContext.BaseDirectory, path)
            : path;
    }

    private void SongChanged(object? sender, SongInfo song)
    {
        eventListener.Publish(new TextStreamEvent {
            Text = $"Песня \"{song.Artist} - {song.Title}\""
        });
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public AudioPlayerService(IStreamEventsService events, IWritableOptions<AudioPlayerServiceConfig> serviceConfig, ILogger<AudioPlayerService> logger)
    {
        eventListener = events;
        log = logger;

        SetConfig(serviceConfig);
    }

    public override void Init()
    {
        base.Init();

        playerCachePath = GetCorrectPaths(config.Value.CachePath);

        // Очистка кэша
        if (Directory.Exists(playerCachePath) && playerCachePath != AppContext.BaseDirectory)
            Directory.GetFiles(playerCachePath, "*.*")
                .ToList()
                .ForEach(File.Delete);

        player = new AudioPlayerBuilder()
            .Configure(new AudioPlayerConfig {
                Volume = config.Value.Volume,
                CachePath = playerCachePath,
                FFMpegPath = GetCorrectPaths(config.Value.FFMpegPath)
            })
            .AddYoutube()
            .Build();

        player.SongChanged += SongChanged;

        UpdateConstraints();
    }

    public override void Dispose()
    {
        if (player != null)
        {
            player.SongChanged -= SongChanged;
        }

        base.Dispose();
    }

    #endregion Основные функции
}