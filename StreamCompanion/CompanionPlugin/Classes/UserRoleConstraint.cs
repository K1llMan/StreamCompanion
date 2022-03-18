﻿using CompanionPlugin.Enums;

using Json.Schema.Generation;

namespace CompanionPlugin.Classes;

public class UserRoleConstraint
{
    [Title("Роли")]
    public UserRole[] Roles { get; set; }
    [Title("Пользователи")]
    public string[] UserNames { get; set; }
}