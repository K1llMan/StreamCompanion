using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

namespace TestPlugin.Classes;

public class TestServiceConfig : ICommandServiceSettings
{
    public bool Enabled { get; set; }
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}