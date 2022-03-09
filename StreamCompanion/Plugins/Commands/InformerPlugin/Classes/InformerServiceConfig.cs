using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

namespace InformerPlugin.Classes;

public class InformerServiceConfig : ICommandServiceSettings
{
    public bool Enabled { get; set; }
    public List<InformerMessage> Messages { get; set; } = new();
    public List<CommandConstraints> Constraints { get; set; } = new();
}