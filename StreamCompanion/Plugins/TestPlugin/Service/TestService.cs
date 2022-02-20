using CompanionPlugin.Classes;
using CompanionPlugin.Enums;

namespace TestPlugin.Service;

public class TestService : CommandService
{
    [BotCommand("!test", UserRole.User)]
    public BotMessage Test(BotMessage message)
    {
        return new BotMessage {
            Text = message.Text,
            Type = MessageType.Success
        };
    }
}