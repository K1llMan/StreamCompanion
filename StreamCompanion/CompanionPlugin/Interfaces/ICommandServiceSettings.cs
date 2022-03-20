using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Models;

namespace CompanionPlugin.Interfaces;

public interface ICommandServiceSettings : IServiceSettings
{
    public List<CommandConstraints> CommandConstraints { get; set; }
}