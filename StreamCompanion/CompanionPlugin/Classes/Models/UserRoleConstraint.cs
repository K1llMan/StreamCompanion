using CompanionPlugin.Enums;

using Json.Schema.Generation;

namespace CompanionPlugin.Classes.Models;

public class UserRoleConstraint
{
    [Title("Роли")]
    [Nullable(true)]

    public UserRole[] Roles { get; set; }
    [Title("Пользователи")]
    [Nullable(true)]

    public string[] UserNames { get; set; }
}