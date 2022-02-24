using System.ComponentModel;
using System.Reflection;

using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandSourceService<T> : ICommandSourceService where T : class, IServiceSettings, new ()
{
    #region Поля

    protected IWritableOptions<T> config;

    #endregion Поля

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

    public event CommandReceivedEventEventHandler? CommandReceivedEvent;

    public Dictionary<string, object> GetDescription()
    {
        DescriptionAttribute desc = GetType().GetCustomAttribute<DescriptionAttribute>();

        return new Dictionary<string, object>
        {
            { "name", desc?.Description ?? string.Empty }
        };
    }

    public virtual void Init()
    {

    }

    #endregion ICommandSourceService
}