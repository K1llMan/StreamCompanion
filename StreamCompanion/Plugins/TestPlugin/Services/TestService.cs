using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;

namespace TestPlugin.Services;

[Description("Тестовый сервис команд")]
public class TestService : CommandService
{
    [BotCommand("!test", UserRole.User)]
    [Description("Тестовая команда для теста тестов")]
    public BotMessage Test(BotMessage message)
    {
        return new BotMessage {
            Text = message.Text,
            Type = MessageType.Success
        };
    }
}