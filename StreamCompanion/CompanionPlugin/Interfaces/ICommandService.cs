using CompanionPlugin.Classes;
using CompanionPlugin.Enums;

namespace CompanionPlugin.Interfaces;

/// <summary>
/// Интерфейс сервиса команд
/// </summary>
public interface ICommandService: IStreamService
{
    BotMessage ProcessCommand(string message, string user, UserRole role);
    void Init();
}