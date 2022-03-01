using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandSourceService<T> : StreamService<T>, ICommandSourceService where T : class, IServiceSettings, new()
{
    #region Вспомогательные функции

    protected BotMessage Received(CommandReceivedArgs args)
    {
        return CommandReceivedEvent?.Invoke(args) 
            ?? new BotMessage {
                Type = MessageType.Error
            };
    }

    #endregion Вспомогательные функции

    #region ICommandSourceService

    public event CommandReceivedEventEventHandler CommandReceivedEvent;

    #endregion ICommandSourceService
}