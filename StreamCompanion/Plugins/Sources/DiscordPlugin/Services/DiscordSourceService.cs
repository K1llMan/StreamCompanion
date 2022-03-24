using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordPlugin.Classes;

using Microsoft.Extensions.Logging;

using StreamEvents.Events;
using StreamEvents.Interfaces;

using MessageType = CompanionPlugin.Enums.MessageType;

namespace DiscordPlugin.Services;

[StreamService("Discord", "discord.json")]
public class DiscordSourceService : CommandSourceService<DiscordSourceServiceConfig>
{
    #region Поля

    private IStreamEventsService eventListener;
    private ILogger<DiscordSourceService> log;

    private DiscordSocketClient? client;
    private IMessageChannel? channel;

    #endregion Поля

    #region Вспомогательные функции

    private async Task ProcessBotResponse(TextStreamEvent textEvent)
    {
        Send(textEvent.Text);
    }

    private UserRole GetUserRole(SocketGuildUser user)
    {
        if (user.GuildPermissions.Administrator)
            return UserRole.Administrator;

        if (user.GuildPermissions.ManageGuild)
            return UserRole.Supermoderator;

        if (user.GuildPermissions.ModerateMembers)
            return UserRole.Moderator;

        return UserRole.User;
    }

    private async Task MessageReceived(SocketMessage messageParam)
    {
        if (channel == null)
            return;

        SocketUserMessage? message = messageParam as SocketUserMessage;
        if (message == null) 
            return;

        SocketCommandContext context = new(client, message);
        if (message.Author.IsBot 
            || context.Channel.Id != channel.Id)
            return;

        SocketGuildUser? user = context.Guild.GetUser(message.Author.Id);

        BotResponseMessage response = Received(new CommandReceivedArgs {
            Message = message.Content,
            User = user.Username,
            Role = GetUserRole(user)
        });

        switch (response)
        {
            case { Type: MessageType.Error }:
                Console.WriteLine(message.Content);
                break;

            case { Type: MessageType.Success } when !string.IsNullOrEmpty(response.Text):
                Send(response.Text);
                break;
        };
    }

    private void Connect()
    {
        client = new DiscordSocketClient();

        client.LoginAsync(TokenType.Bot, config.Value.Token)
            .GetAwaiter()
            .GetResult();

        client.StartAsync()
            .GetAwaiter()
            .GetResult();

        client.MessageReceived += MessageReceived;
        client.Ready += async () =>
        {
            channel = client.GetChannel(config.Value.ChannelId) as IMessageChannel;
            Send("Подключён.");
        };
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public DiscordSourceService(IStreamEventsService events, IWritableOptions<DiscordSourceServiceConfig> serviceConfig, ILogger<DiscordSourceService> logger)
    {
        eventListener = events;
        log = logger;
        SetConfig(serviceConfig);
    }

    public void Send(string message)
    {
        channel?. SendMessageAsync(message);
    }

    public override void Init()
    {
        if (!config.Value.IsProperlyConfigured())
            return;

        if (config.Value.SubscribeToEvents)
        {
            eventListener.Subscribe<TextStreamEvent>(ProcessBotResponse);
        }

        Connect();
    }

    public override void Dispose()
    {
        if (config.Value.SubscribeToEvents)
        {
            eventListener.Unsubscribe<TextStreamEvent>(ProcessBotResponse);
        }

        client?.LogoutAsync()
            .GetAwaiter()
            .GetResult();

        base.Dispose();
    }

    #endregion Основные функции
}