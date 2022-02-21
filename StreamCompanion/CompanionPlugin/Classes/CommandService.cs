﻿using System.ComponentModel;
using System.Reflection;

using CompanionPlugin.Enums;
using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandService : ICommandService
{
    #region Поля

    protected Dictionary<string, CommandInfo> commands = new ();

    #endregion Поля

    #region Вспомогательные функции

    private void AddCommand(CommandInfo info)
    {
        if (!commands.ContainsKey(info.Command))
        {
            commands.Add(info.Command, info);
        }
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public BotMessage ProcessCommand(string message, string user, UserRole role)
    {
        message = message.Trim();

        if (message.StartsWith("!"))
        {
            string command = message.GetMatches(@"^!\w+").FirstOrDefault();
            string data = message.GetMatches(@"(?<=\b[\s]).+").FirstOrDefault();

            if (string.IsNullOrEmpty(command?.TrimStart('!')) || !commands.ContainsKey(command))
                return new BotMessage {
                    Type = MessageType.NotCommand
                };

            return commands[command].Handler.Invoke(new BotMessage {
                Command = command,
                Text = data,
                Role = role,
                User = user
            });
        }

        return new BotMessage
        {
            Type = MessageType.NotCommand
        };
    }

    public virtual void Init()
    {
        GetType().GetMethods()
            .Where(m => m.GetCustomAttribute<BotCommandAttribute>() != null)
            .ToList()
            .ForEach(m => {
                BotCommandAttribute command = m.GetCustomAttribute<BotCommandAttribute>();
                DescriptionAttribute desc = m.GetCustomAttribute<DescriptionAttribute>();

                if (command != null)
                    AddCommand(new CommandInfo {
                        Command = command.Command,
                        Description = desc?.Description,
                        Role = command.Role,
                        Handler = m.CreateDelegate<MessageHandler>(this)
                    });
            });
    }

    public string GetDescription()
    {
        DescriptionAttribute desc = GetType().GetCustomAttribute<DescriptionAttribute>();

        return $"Сервис \"{desc?.Description}\"\n" +
            "\tКоманды:\n" +
            string.Join("\n", commands.Values.Select(c => $"\t\t\"{c.Command}\": {c.Description}"));
    }

    #endregion Основные функции
}