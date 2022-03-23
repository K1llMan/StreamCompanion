using System.ComponentModel;

using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using InformerPlugin.Classes;

using StreamEvents.Events;
using StreamEvents.Interfaces;

namespace InformerPlugin.Services;

[StreamService("Сервис-информатор", "informer.json")]
public class InformerService : CommandService<InformerServiceConfig>
{
    #region Поля

    private IStreamEventsService events;
    private CancellationTokenSource cancellationTokenSource;

    private CountInfo? counter;

    #endregion Поля

    #region Команды

    [BotCommand("!newCount")]
    [Description("Создать новый счётчик")]
    public BotResponseMessage NewCount(BotMessage message)
    {
        if (string.IsNullOrEmpty(message.Text))
            return new BotResponseMessage {
                Text = "Не задано имя счётчика",
                Type = MessageType.Error
            };

        counter = new CountInfo {
            Name = message.Text
        };

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    [BotCommand("!addCount")]
    [Description("Добавить значение")]
    public BotResponseMessage Count(BotMessage message)
    {
        if (counter == null)
            return new BotResponseMessage {
                Type = MessageType.Error
            };

        int.TryParse(message.Text, out int num);

        if (num == 0)
            num = 1;

        counter.Count += num;

        return new BotResponseMessage {
            Text = $"{counter.Name}: {counter.Count}",
            Type = MessageType.Success
        };
    }

    [BotCommand("!resetCount")]
    [Description("Сбросить счётчик")]
    public BotResponseMessage ResetCount(BotMessage message)
    {
        if (counter == null)
            return new BotResponseMessage {
                Type = MessageType.Error
            };

        counter.Count = 0;

        return new BotResponseMessage {
            Text = $"{counter.Name}: {counter.Count}",
            Type = MessageType.Success
        };
    }

    [BotCommand("!count")]
    [Description("Получить значение счётчика")]
    public BotResponseMessage GetCount(BotMessage message)
    {
        if (counter == null)
            return new BotResponseMessage {
                Type = MessageType.Error
            };

        return new BotResponseMessage {
            Text = $"{counter.Name}: {counter.Count}",
            Type = MessageType.Success
        };
    }

    #endregion Команды

    #region Вспомогательные функции

    private void SendText(string text)
    {
        events.Publish(new TextStreamEvent {
            Text = text
        });
    }

    private BotResponseMessage SendMessage(BotMessage message, string text)
    {
        SendText(text);

        return new BotResponseMessage {
            Type = MessageType.Success
        };
    }

    private void StartSend(InformerMessage message, CancellationToken token)
    {
        // Пропуск сообщений, которые не надо дублировать по таймауту
        if (message.Timeout == -1)
            return;

        Task.Run(() => {
            do
            {
                Thread.Sleep(message.Timeout);
                SendText(message.Text);
            } while (!token.IsCancellationRequested);
        });
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public InformerService(IStreamEventsService eventsService, IWritableOptions<InformerServiceConfig> serviceConfig)
    {
        events = eventsService;

        SetConfig(serviceConfig);
    }

    public override void Init()
    {
        if (!config.Value.Enabled)
            return;

        base.Init();

        cancellationTokenSource = new CancellationTokenSource();

        foreach (InformerMessage message in config.Value.Messages)
        {
            StartSend(message, cancellationTokenSource.Token);

            foreach (string alias in message.Aliases)
                if (!commands.ContainsKey(alias))
                    commands.Add(alias, new CommandInfo {
                        Command = alias,
                        Description = message.Text,
                        Handler = msg => SendMessage(msg, message.Text)
                    });
        }

        UpdateConstraints();

    }

    public override void Dispose()
    {
        cancellationTokenSource?.Cancel(false);

        base.Dispose();
    }

    #endregion Основные функции
}