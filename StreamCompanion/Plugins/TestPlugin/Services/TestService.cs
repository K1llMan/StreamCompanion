using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Logging;

using StreamEvents.Events;
using StreamEvents.Interfaces;

using TestPlugin.Classes;

namespace TestPlugin.Services;

[Description("Тестовый сервис команд")]
public class TestService : CommandService<TestServiceConfig>
{
    #region Поля

    private ILogger<TestService> log;
    private IStreamEventsService eventListener;

    #endregion Поля

    #region Команды

    [BotCommand("!test")]
    [Description("Тестовая команда для теста тестов")]
    public BotMessage Test(BotMessage message)
    {
        eventListener.Publish(new TextStreamEvent {
            Text = message.Text
        });

        return new BotMessage {
            Text = $"Ты кто такой? Шо ты мне пришешь \"{message.Text}\"?! Нахуй пшёл!",
            Type = MessageType.Success
        };
    }

    [BotCommand("!adminTest")]
    [Description("Тест команды администратора")]
    public BotMessage AdminTest(BotMessage message)
    {
        return new BotMessage
        {
            Text = $"Привет, братюня-админ {message.User}!",
            Type = MessageType.Success
        };
    }

    [BotCommand("!modTest")]
    [Description("Тест команды модератора")]
    public BotMessage ModeratorTest(BotMessage message)
    {
        return new BotMessage
        {
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

        eventListener.Subscribe<TextStreamEvent>(processTextEvent);
        eventListener.Subscribe<TextStreamEvent>(processTextEvent1);
        eventListener.Subscribe<TextStreamEvent>(processTextEvent2);

        SetConfig(serviceConfig);
    }

    public override void Dispose()
    {
        eventListener.Unsubscribe<TextStreamEvent>(processTextEvent);
        eventListener.Unsubscribe<TextStreamEvent>(processTextEvent1);
        eventListener.Unsubscribe<TextStreamEvent>(processTextEvent2);
    }
}