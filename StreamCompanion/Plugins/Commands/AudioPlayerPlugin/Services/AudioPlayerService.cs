using System.ComponentModel;

using AudioPlayerPlugin.Classes;

using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Logging;

using NAudioPlayer;
using NAudioPlayer.Classes;
using NAudioPlayer.Classes.Providers;

using StreamEvents.Events;
using StreamEvents.Interfaces;

namespace AudioPlayerPlugin.Services;

[StreamService("Музыкальный плеер", "audioPlayer.json")]
public class AudioPlayerService : CommandService<AudioPlayerServiceConfig>
{
    #region Поля

    private IStreamEventsService eventListener;
    private ILogger<AudioPlayerService> log;

    private string playerCachePath;
    private AudioPlayer? player;

    #endregion Поля

    #region Команды

    [BotCommand("!djAdd")]
    [Description("Добавить песню")]
    public BotResponseMessage Add(BotMessage message)
    {
        player?.AddFromProvider(message.Text);

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djPlay")]
    [Description("Запустить проигрыватель")]
    public BotResponseMessage Play(BotMessage message)
    {
        player?.Play();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djPause")]
    [Description("Поставить проигрыватель на паузу")]
    public BotResponseMessage Pause(BotMessage message)
    {
        player?.Pause();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djStop")]
    [Description("Остановить проигрыватель")]
    public BotResponseMessage Stop(BotMessage message)
    {
        player?.Stop();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djVolume")]
    [Description("Установить уровень громкости")]
    public BotResponseMessage Volume(BotMessage message)
    {
        if (!float.TryParse(message.Text.Replace(".", ","), out float volume))
            return new BotResponseMessage {
                Text = "Неверное значение громкости",
                Type = MessageType.Error
            };

        player?.Volume(volume);

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djNext")]
    [Description("Следующий трек")]
    public BotResponseMessage Next(BotMessage message)
    {
        player?.Next();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djPrev")]
    [Description("Предыдущий трек")]
    public BotResponseMessage Prev(BotMessage message)
    {
        player?.Previous();

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!djWhat")]
    [Description("Информация о треке")]
    public BotResponseMessage What(BotMessage message)
    {
        if (player?.CurrengSong == null)
            return new BotResponseMessage {
                Text = $"Песня отсутствует",
                Type = MessageType.Error
            };

        string title = string.Join(" - ", new[] { player.CurrengSong.Artist, player.CurrengSong.Title }
            .Where(s => !string.IsNullOrEmpty(s))
        );

        return new BotResponseMessage {
            Text = $"Песня \"{title}\"",
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

    private void SongChanged(object? sender, SongInfo? song)
    {
        if (song == null)
            return;

        string title = string.Join(" - ", new[] { song.Artist, song.Title }
            .Where(s => !string.IsNullOrEmpty(s))
        );

        eventListener.Publish(new TextStreamEvent {
            Text = $"Песня \"{title}\""
        });
    }

    private AudioPlayer InitPlayer(string cachePath)
    {
        AudioPlayerBuilder playerBuilder = new AudioPlayerBuilder()
            .Configure(new AudioPlayerConfig {
                Volume = config.Value.Volume,
                CachePath = cachePath,
                FFMpegPath = GetCorrectPaths(config.Value.FFMpegPath)
            })
            .AddYoutube(config.Value.Providers?.Youtube);

        if (config.Value.Providers?.Local != null)
            playerBuilder.AddLocal(config.Value.Providers?.Local);

        AudioPlayer songPlayer = playerBuilder.Build();

        songPlayer.SongChanged += SongChanged;

        return songPlayer;
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
        if (!config.Value.IsProperlyConfigured())
            return;

        base.Init();

        playerCachePath = GetCorrectPaths(config.Value.CachePath);

        // Очистка кэша
        if (Directory.Exists(playerCachePath) && playerCachePath != AppContext.BaseDirectory)
            Directory.GetFiles(playerCachePath, "*.*")
                .ToList()
                .ForEach(File.Delete);

        player = InitPlayer(playerCachePath);

        UpdateConstraints();
    }

    public override void Dispose()
    {
        if (player != null)
            player.SongChanged -= SongChanged;

        base.Dispose();
    }

    #endregion Основные функции
}