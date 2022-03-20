using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Logging;

using StreamEvents.Events;
using StreamEvents.Interfaces;

using TestPlugin.Classes;

namespace TestPlugin.Services;

[StreamService("Тестовый сервис команд", "test.json")]
public class TestService : CommandService<TestServiceConfig>
{
    #region Поля

    private ILogger<TestService> log;
    private IStreamEventsService eventListener;

    #endregion Поля

    #region Команды

    [BotCommand("!test", "!тест")]
    [Description("Тестовая команда для теста тестов")]
    public BotResponseMessage Test(BotMessage message)
    {
        eventListener.Publish(new TextStreamEvent {
            Text = message.Text
        });

        return new BotResponseMessage {
            Text = $"Ты кто такой? Шо ты мне пришешь \"{message.Text}\"?! Нахуй пшёл!",
            Type = MessageType.Success
        };
    }

    [BotCommand("!adminTest")]
    [Description("Тест команды администратора")]
    public BotResponseMessage AdminTest(BotMessage message)
    {
        return new BotResponseMessage {
            Text = $"Привет, братюня-админ {message.User}!",
            Type = MessageType.Success
        };
    }

    [BotCommand("!modTest")]
    [Description("Тест команды модератора")]
    public BotResponseMessage ModeratorTest(BotMessage message)
    {
        return new BotResponseMessage {
            Text = $"Привет, братюня-модератор {message.User}!",
            Type = MessageType.Success
        };
    }

    #endregion Команды

    #region Вспомогательные функции

    private async Task processTextEvent(TextStreamEvent textEvent)
    {
        log.LogInformation(textEvent.Text);
    }

    private async Task processTextEvent1(TextStreamEvent textEvent)
    {
        log.LogInformation(textEvent.Text);
    }

    private async Task processTextEvent2(TextStreamEvent textEvent)
    {
        log.LogInformation(textEvent.Text);
    }

    #endregion Вспомогательные функции

    public TestService(IStreamEventsService events, IWritableOptions<TestServiceConfig> serviceConfig, ILogger<TestService> logger)
    {
        log = logger;
        eventListener = events;

        SetConfig(serviceConfig);
    }

    public override void Init()
    {
        if (!config.Value.Enabled)
            return;

        base.Init();

        eventListener.Subscribe<TextStreamEvent>(processTextEvent);
        eventListener.Subscribe<TextStreamEvent>(processTextEvent1);
        eventListener.Subscribe<TextStreamEvent>(processTextEvent2);
    }

    public override void Dispose()
    {
        base.Dispose();

        eventListener.Unsubscribe<TextStreamEvent>(processTextEvent);
        eventListener.Unsubscribe<TextStreamEvent>(processTextEvent1);
        eventListener.Unsubscribe<TextStreamEvent>(processTextEvent2);
    }
}