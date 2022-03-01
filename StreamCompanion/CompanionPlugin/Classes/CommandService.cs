using System.ComponentModel;
using System.Reflection;

using CompanionPlugin.Enums;
using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandService<T> : StreamService<T>, ICommandService where T : class, IServiceSettings, new()
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
        if (config == null)
            return new BotMessage
            {
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

            if (string.IsNullOrEmpty(command?.TrimStart('!')) || !commands.ContainsKey(command) || commands[command].Role < role)
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
                        Role = command.Role,
                        Handler = m.CreateDelegate<MessageHandler>(this)
                    });
            });
    }

    public override Dictionary<string, object> GetDescription()
    {
        Dictionary<string, object> desc = base.GetDescription();
        desc.Add("commands", commands.Values.Select(c => new Dictionary<string, object> {
            { "command", c.Command },
            { "description", c.Description },
            { "role", c.Role },
        }));

        return desc;
    }

    #endregion Основные функции
}