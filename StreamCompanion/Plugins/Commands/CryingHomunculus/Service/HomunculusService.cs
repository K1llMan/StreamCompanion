using System.ComponentModel;

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
    private ILogger<HomunculusService> log;

    #endregion Поля

    #region Вспомогательные функции

    private BotMessage MessageHandler(BotMessage message, string path)
    {
        path = Path.Combine(audioPath, path);

        if (File.Exists(path))
            using (AudioFileReader audioFile = new(path))
                using (WaveOutEvent outputDevice = new())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }

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
        audioPath = !Directory.Exists(config.Value.AudioPath) 
            ? Path.Combine(AppContext.BaseDirectory, config.Value.AudioPath)
            : config.Value.AudioPath;

        foreach (VoiceCommand command in config.Value.Commands)
        {
            if (!commands.ContainsKey(command.Command))
                commands.Add(command.Command, new CommandInfo {
                    Command = command.Command,
                    Description = command.Description,
                    Role = command.Role,
                    Handler = msg => MessageHandler(msg, command.FilePath)
                });
        };
    }

    #endregion Основные функции
}