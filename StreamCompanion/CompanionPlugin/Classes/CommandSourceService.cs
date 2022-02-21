﻿using System.ComponentModel;
using System.Reflection;

using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandSourceService: ICommandSourceService
{
    #region Вспомогательные функции

    protected BotMessage Received(CommandReceivedArgs args)
    {
        return CommandReceivedEvent?.Invoke(args) 
            ?? new BotMessage {
                Type = MessageType.Error
            };
    }

    #endregion Вспомогательные функции

    #region ICommandSourceService

    public event CommandReceivedEventEventHandler? CommandReceivedEvent;

    public string GetDescription()
    {
        DescriptionAttribute desc = GetType().GetCustomAttribute<DescriptionAttribute>();

        return $"Сервис \"{desc?.Description}\"";
    }

    #endregion ICommandSourceService
}