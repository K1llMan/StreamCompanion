using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

namespace InformerPlugin.Classes;

public class InformerServiceConfig : ServiceSettings, ICommandServiceSettings
{
    public List<InformerMessage> Messages { get; set; } = new();
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}