using CompanionPlugin.Classes.Models;
using CompanionPlugin.Enums;

namespace CompanionPlugin.Interfaces;

/// <summary>
/// Интерфейс сервиса команд
/// </summary>
public interface ICommandService: IStreamService
{
    BotResponseMessage ProcessCommand(string message, string user, UserRole role);
}