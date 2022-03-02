using CompanionPlugin.Classes;

namespace CompanionPlugin.Interfaces;

public interface ICommandServiceSettings : IServiceSettings
{
    public List<CommandConstraints> Constraints { get; set; }
}