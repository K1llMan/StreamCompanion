using CompanionPlugin.Enums;

namespace CompanionPlugin.Classes;

public class CommandConstraints
{
    public string Command { get; set; }
    public bool Enabled { get; set; }
    public UserRole[] Roles { get; set; }
    public string[] UserNames { get; set; }
}