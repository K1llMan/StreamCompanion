using CompanionPlugin.Classes;

namespace CompanionPlugin.Interfaces;

public interface ICommandServiceSettings : IServiceSettings
{
    public List<CommandConstraints> CommandConstraints { get; set; }
}