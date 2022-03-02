using System.ComponentModel;
using System.Reflection;

using CompanionPlugin.Enums;
using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandService<T> : StreamService<T>, ICommandService where T : class, ICommandServiceSettings, new()
{
    #region Поля

    protected Dictionary<string, CommandInfo> commands = new();

    #endregion Поля

    #region Вспомогательные функции

    private void AddCommand(CommandInfo info)
    {
        if (!commands.ContainsKey(info.Command))
        {
            commands.Add(info.Command, info);
        }
    }

    private bool IsCorrectCommand(string command, string user, UserRole role)
    {
        if (string.IsNullOrEmpty(command?.TrimStart('!')) || !commands.ContainsKey(command))
            return false;

        List<CommandConstraints> constraints = config.Value.Constraints;

        CommandConstraints constraint = constraints.FirstOrDefault(c => c.Command == command);
        if (constraint == null)
            return true;

        if (constraint is not { Enabled: true })
            return false;

        if (constraint.Roles is { Length: > 0 })
            if (!constraint.Roles.Contains(role))
                return false;

        if (constraint.UserNames is { Length: > 0 })
            if (!constraint.UserNames.Contains(user))
                return false;

        return true;
    }

    /// <summary>
    /// Добавление ограничения на команду, если оно отсутствовало
    /// </summary>
    protected void UpdateConstraints()
    {
        List<CommandConstraints> constraints = config.Value.Constraints;

        Func<string, bool> hasConstr = c => constraints
            .Any(con => con.Command == c);

        foreach (string command in commands.Keys)
        {
            if (hasConstr(command))
                continue;

            constraints.Add(new CommandConstraints {
                Enabled = true,
                Command = command
            });
        }

        config.Update(c => {
            c.Constraints = constraints;
            return c;
        });
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public BotMessage ProcessCommand(string message, string user, UserRole role)
    {
        if (config == null)
            return new BotMessage {
                Type = MessageType.NotCommand
            };

        if (!config.Value.Enabled)
            return new BotMessage {
                Type = MessageType.NotCommand
            };

        message = message.Trim();

        if (message.StartsWith("!"))
        {
            string command = message.GetMatches(@"^!\w+").FirstOrDefault();
            string data = message.GetMatches(@"(?<=\b[\s]).+").FirstOrDefault();

            if (!IsCorrectCommand(command, user, role))
                return new BotMessage {
                    Type = MessageType.NotCommand
                };

            if (command != null)
                return commands[command].Handler.Invoke(new BotMessage {
                    Command = command,
                    Text = data,
                    Role = role,
                    User = user
                });
        }

        return new BotMessage {
            Type = MessageType.NotCommand
        };
    }

    public override void Init()
    {
        commands.Clear();

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
                        Handler = m.CreateDelegate<MessageHandler>(this)
                    });
            });

        UpdateConstraints();
    }

    public override Dictionary<string, object> GetDescription()
    {
        Dictionary<string, object> desc = base.GetDescription();
        desc.Add("commands", commands.Values.Select(c =>
        {
            CommandConstraints constraint = config.Value.Constraints.FirstOrDefault(con => con.Command == c.Command);

            return new Dictionary<string, object> {
                { "command", c.Command },
                { "description", c.Description },
                { "roles", constraint?.Roles },
                { "userNames", constraint?.UserNames },
            };
        }));

        return desc;
    }

    #endregion Основные функции
}