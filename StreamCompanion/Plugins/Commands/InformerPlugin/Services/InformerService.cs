using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using InformerPlugin.Classes;

using StreamEvents.Events;
using StreamEvents.Interfaces;

namespace InformerPlugin.Services;

[Description("Сервис-информатор")]
public class InformerService : CommandService<InformerServiceConfig>
{
    #region Поля

    private IStreamEventsService events;

    private CancellationTokenSource cancellationTokenSource = new();

    #endregion Поля

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

        foreach (InformerMessage message in config.Value.Messages)
        {
            StartSend(message, cancellationTokenSource.Token);

            if (!commands.ContainsKey(message.Command))
                commands.Add(message.Command, new CommandInfo {
                    Command = message.Command,
                    Description = message.Text,
                    Handler = msg => SendMessage(msg, message.Text)
                });
        }

        UpdateConstraints();
    }

    public override void Dispose()
    {
        cancellationTokenSource.Cancel(false);

        base.Dispose();
    }

    #endregion Основные функции
}