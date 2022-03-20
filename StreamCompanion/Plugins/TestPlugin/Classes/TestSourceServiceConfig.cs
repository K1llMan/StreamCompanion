using CompanionPlugin.Classes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace TestPlugin.Classes;

public class TestSourceServiceConfig : ServiceSettings, ICommandSourceServiceSettings
{
    [Title("Подписка на события")]
    public bool SubscribeToEvents { get; set; }
}