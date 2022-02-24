using CompanionPlugin.Interfaces;

namespace TestPlugin.Classes;

public class TestSourceServiceConfig : IServiceSettings
{
    public bool Enabled { get; set; }
}