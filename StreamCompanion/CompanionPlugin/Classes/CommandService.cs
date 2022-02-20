using CompanionPlugin.Enums;
using CompanionPlugin.Extensions;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes;

public class CommandService : ICommandService
{
    #region Поля

    private delegate BotMessage MessageHandler(BotMessage message);

    private Dictionary<string, MessageHandler> commands = new ();

    #endregion Поля

    #region Основные функции

    public BotMessage ProcessCommand(string message, UserRole role)
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

            return new BotMessage {
                Text = data,
                Type = MessageType.Success
            };
        }

        return new BotMessage
        {
            Type = MessageType.NotCommand
        };
    }

    public void Init()
    {
        throw new NotImplementedException();
    }

    #endregion Основные функции
}