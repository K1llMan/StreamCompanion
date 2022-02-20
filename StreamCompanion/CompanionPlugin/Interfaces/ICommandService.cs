using CompanionPlugin.Classes;
using CompanionPlugin.Enums;

namespace CompanionPlugin.Interfaces;

public interface ICommandService
{
    BotMessage ProcessCommand(string messsage, UserRole role);
    void Init();
}