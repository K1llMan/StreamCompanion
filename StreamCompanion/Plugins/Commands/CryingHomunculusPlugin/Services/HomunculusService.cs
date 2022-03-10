﻿using System.ComponentModel;
using System.Diagnostics;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using CryingHomunculusPlugin.Classes;

using Microsoft.Extensions.Logging;

using NAudio.Wave;

namespace CryingHomunculusPlugin.Services;

[Description("Орущий гомункул")]
public class HomunculusService : CommandService<HomunculusServiceConfig>
{
    #region Поля

    private string audioPath;
    private string speechPath;
    private string speechCachePath;

    private DateTime nextTime = DateTime.Now;

    private ILogger<HomunculusService> log;

    #endregion Поля

    #region Команды

    [BotCommand("!озвучить")]
    [Description("Озвучить текст из команды. Голоса можно найти на https://rhvoice.su/voices/")]
    public BotResponseMessage SayText(BotMessage message)
    {
        if (string.IsNullOrEmpty(message.Text))
            return new BotResponseMessage {
                Type = MessageType.Error
            };

        if (!IsReadyToPlay())
            return new BotResponseMessage {
                Type = MessageType.Error
            };

        PlaySound(TextToSpeech(message.Text));

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    #endregion Команды

    #region Вспомогательные функции

    #region Text-To-Speech

    private string GetSpeechParams(string text, string path)
    {
        List<string> parameters = new() {
            $"-t \"{text}\"",
            $"-w \"{path}\"",
        };

        if (!string.IsNullOrEmpty(config.Value.Speech.Voice))
            parameters.Add($"-n \"{config.Value.Speech.Voice}\"");

        return string.Join(" ", parameters);
    }

    private string TextToSpeech(string text)
    {
        if (!Directory.Exists(speechCachePath))
            Directory.CreateDirectory(speechCachePath);

        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        string path = Path.Combine(speechCachePath, $"{fileName}.wav");

        Process process = new() {
            StartInfo = new ProcessStartInfo(
                Path.Combine(speechPath, "balcon.exe"), 
                GetSpeechParams(text, path)
            )
        };

        process.Start();
        process.WaitForExit();

        return path;
    }

    #endregion Text-To-Speech

    private string GetCorrectPaths(string path)
    {
        return !Directory.Exists(path)
            ? Path.Combine(AppContext.BaseDirectory, path)
            : path;
    }

    private void ChangeVolume(AudioFileReader reader)
    {
        float max = 0;
        float[] buffer = new float[reader.WaveFormat.SampleRate];
        int read;
        do
        {
            read = reader.Read(buffer, 0, buffer.Length);
            for (int n = 0; n < read; n++)
            {
                float abs = Math.Abs(buffer[n]);
                if (abs > max) 
                    max = abs;
            }
        } while (read > 0);

        if (max == 0 || max > 1.0f)
            throw new InvalidOperationException("File cannot be normalized");

        // rewind and amplify
        reader.Position = 0;
        reader.Volume = 1.0f / max;
    }

    private void PlaySound(string path)
    {
        nextTime = DateTime.Now.AddMilliseconds(config.Value.Timeout);

        Task.Run(() => {
            using AudioFileReader audioFile = new(path);
            ChangeVolume(audioFile);

            audioFile.Volume *= config.Value.Volume;

            using WaveOutEvent outputDevice = new();

            outputDevice.Init(audioFile);
            outputDevice.Play();

            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }
        });
    }

    private bool IsReadyToPlay()
    {
        return DateTime.Now > nextTime;
    }

    private BotResponseMessage PlayCommand(BotMessage message, string path)
    {
        if (!IsReadyToPlay())
            return new BotResponseMessage {
                Type = MessageType.Error
            };

        path = Path.Combine(audioPath, path);

        if (File.Exists(path))
            PlaySound(path);

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public HomunculusService(IWritableOptions<HomunculusServiceConfig> serviceConfig, ILogger<HomunculusService> logger)
    {
        log = logger;

        SetConfig(serviceConfig);
    }

    public override void Init()
    {
        base.Init();

        audioPath = GetCorrectPaths(config.Value.AudioPath);
        speechPath = GetCorrectPaths(config.Value.Speech.Path);
        speechCachePath = GetCorrectPaths(config.Value.Speech.CachePath);

        // Очистка кэша
        if (Directory.Exists(speechCachePath) && speechCachePath != AppContext.BaseDirectory)
            Directory.GetFiles(speechCachePath, "*.*")
                .ToList()
                .ForEach(File.Delete);

        foreach (VoiceCommand command in config.Value.Commands)
            foreach (string alias in command.Aliases)
                if (!commands.ContainsKey(alias))
                    commands.Add(alias, new CommandInfo {
                        Command = alias,
                        Description = command.Description,
                        Handler = msg => PlayCommand(msg, command.FilePath)
                    });

        UpdateConstraints();
    }

    #endregion Основные функции
}