using System.ComponentModel;
using System.Diagnostics;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using CryingHomunculus.Classes;

using Microsoft.Extensions.Logging;

using NAudio.Wave;

namespace CryingHomunculus.Service;

[Description("Орущий гомункул")]
public class HomunculusService : CommandService<HomunculusServiceConfig>
{
    #region Поля

    private string audioPath;
    private string speechPath;
    private string speechCachePath;
    private ILogger<HomunculusService> log;

    #endregion Поля

    #region Команды

    [BotCommand("!озвучить", UserRole.Moderator)]
    [Description("Озвучить текст из команды")]
    public BotMessage SayText(BotMessage message)
    {
        PlaySound(TextToSpeech(message.Text));

        return new BotMessage
        {
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

    private string TextToSpeech(string text)
    {
        if (!Directory.Exists(speechCachePath))
            Directory.CreateDirectory(speechCachePath);

        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        string path = Path.Combine(speechCachePath, $"{fileName}.wav");

        Process process = new() {
            StartInfo = new ProcessStartInfo(Path.Combine(speechPath, "balcon.exe"),
                $"-t \"{text}\" -w \"{path}\""
            )
        };

        process.Start();
        process.WaitForExit();

        return path;
    }

    private void PlaySound(string path)
    {
        using AudioFileReader audioFile = new(path);
        using WaveOutEvent outputDevice = new();

        outputDevice.Init(audioFile);
        outputDevice.Play();

        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            Thread.Sleep(1000);
        }
    }

    private BotMessage PlayCommand(BotMessage message, string path)
    {
        path = Path.Combine(audioPath, path);

        if (File.Exists(path))
            PlaySound(path);

        return new BotMessage {
            Type = MessageType.Success
        };
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public HomunculusService(IWritableOptions<HomunculusServiceConfig> serviceConfig, ILogger<HomunculusService> logger)
    {
        config = serviceConfig;
        log = logger;
    }

    public override void Init()
    {
        base.Init();

        audioPath = GetCorrectPaths(config.Value.AudioPath);
        speechPath = GetCorrectPaths(config.Value.Speech.Path);
        speechCachePath = GetCorrectPaths(config.Value.Speech.CachePath);

        foreach (VoiceCommand command in config.Value.Commands)
        {
            if (!commands.ContainsKey(command.Command))
                commands.Add(command.Command, new CommandInfo {
                    Command = command.Command,
                    Description = command.Description,
                    Role = command.Role,
                    Handler = msg => PlayCommand(msg, command.FilePath)
                });
        };
    }

    #endregion Основные функции
}