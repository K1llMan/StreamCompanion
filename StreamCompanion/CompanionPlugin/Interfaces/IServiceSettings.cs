using Json.Schema;

namespace CompanionPlugin.Interfaces;

public interface IServiceSettings
{
    bool Enabled { get; set; }
    public JsonSchema Schema { get; set; }

    public bool Validate(string data);
}