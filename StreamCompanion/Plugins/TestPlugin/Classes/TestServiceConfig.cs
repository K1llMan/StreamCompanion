using CompanionPlugin.Classes;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace TestPlugin.Classes;

public class TestServiceConfig : ServiceSettings, ICommandServiceSettings
{
    [Title("Ограничение для команд")]
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}