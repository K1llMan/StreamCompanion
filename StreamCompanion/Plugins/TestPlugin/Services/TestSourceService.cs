using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using TestPlugin.Classes;

namespace TestPlugin.Services;

[Description("Тестовый сервис-источник")]
public class TestSourceService: CommandSourceService<TestSourceServiceConfig>
{
    #region Основные функции

    public BotMessage AddMessage(string message, UserRole role)
    {
        return Received(new CommandReceivedArgs {
            Message = message,
            User = "Test user",
            Role = role
        });
    }

    #endregion Основные функции
}