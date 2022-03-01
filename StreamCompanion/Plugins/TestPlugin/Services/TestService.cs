using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using TestPlugin.Classes;

namespace TestPlugin.Services;

[Description("Тестовый сервис команд")]
public class TestService : CommandService<TestServiceConfig>
{
    [BotCommand("!test", UserRole.User)]
    [Description("Тестовая команда для теста тестов")]
    public BotMessage Test(BotMessage message)
    {
        return new BotMessage {
            Text = $"Ты кто такой? Шо ты мне пришешь \"{message.Text}\"?! Нахуй пшёл!",
            Type = MessageType.Success
        };
    }

    [BotCommand("!adminTest", UserRole.Administrator)]
    [Description("Тест команды администратора")]
    public BotMessage AdminTest(BotMessage message)
    {
        return new BotMessage
        {
            Text = $"Привет, братюня-админ {message.User}!",
            Type = MessageType.Success
        };
    }

    [BotCommand("!modTest", UserRole.Moderator)]
    [Description("Тест команды модератора")]
    public BotMessage ModeratorTest(BotMessage message)
    {
        return new BotMessage
        {
            Text = $"Привет, братюня-модератор {message.User}!",
            Type = MessageType.Success
        };
    }

    public TestService(IWritableOptions<TestServiceConfig> serviceConfig)
    {
        SetConfig(serviceConfig);
    }
}