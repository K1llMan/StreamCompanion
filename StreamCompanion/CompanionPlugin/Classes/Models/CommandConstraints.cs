using Json.Schema.Generation;

namespace CompanionPlugin.Classes.Models;

public class CommandConstraints
{
    [Title("Команда")]
    public string Command { get; set; }
    [Title("Доступность")]
    public bool Enabled { get; set; }
    [Title("Ограничение")]
    public List<UserRoleConstraint> Constraints { get; set; }
}