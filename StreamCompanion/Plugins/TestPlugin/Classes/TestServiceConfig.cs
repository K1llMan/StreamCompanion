using CompanionPlugin.Interfaces;

namespace TestPlugin.Classes;

public class TestServiceConfig : IServiceSettings
{
    public bool Enabled { get; set; }
}