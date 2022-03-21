using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using TestPlugin.Classes;

namespace TestPlugin.Services;

[StreamService("Тестовый сервис-источник", "testSource.json")]
public class TestSourceService: CommandSourceService<TestSourceServiceConfig>
{
    #region Основные функции

    public BotResponseMessage AddMessage(string message, UserRole role)
    {
        return Received(new CommandReceivedArgs {
            Message = message,
            User = "Test user",
            Role = role
        });
    }

    #endregion Основные функции

    #region Основные функции

    public TestSourceService(IWritableOptions<TestSourceServiceConfig> serviceConfig)
    {
        SetConfig(serviceConfig);
    }

    #endregion Основные функции
}