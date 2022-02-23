using CompanionPlugin.Classes;
using CompanionPlugin.Enums;

namespace CompanionPlugin.Interfaces;

public delegate BotMessage CommandReceivedEventEventHandler(CommandReceivedArgs args);

public class CommandReceivedArgs
{
    public string Message { get; set; }
    public string User { get; set; }
    public UserRole Role { get; set; }
}

/// <summary>
/// Интерфейс сервиса-источника
/// </summary>
public interface ICommandSourceService : IStreamService
{
    event CommandReceivedEventEventHandler CommandReceivedEvent;
}

