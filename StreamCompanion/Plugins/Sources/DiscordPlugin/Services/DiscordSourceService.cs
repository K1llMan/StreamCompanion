using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Discord;
using Discord.WebSocket;

using DiscordPlugin.Classes;

using Microsoft.Extensions.Logging;

using StreamEvents.Events;
using StreamEvents.Interfaces;

namespace DiscordPlugin.Services;

[StreamService("Discord", "discord.json")]
public class DiscordSourceService : CommandSourceService<DiscordSourceServiceConfig>
{
    #region Поля

    private IStreamEventsService eventListener;
    private ILogger<DiscordSourceService> log;
    private DiscordSocketClient? client;

    #endregion Поля

    #region Вспомогательные функции


    private UserRole GetUserRole(object message)
    {
        /*
        if (message.IsBroadcaster)
            return UserRole.Administrator;
        if (message.IsModerator)
            return UserRole.Moderator;
        */
        return UserRole.User;
    }

    private async Task ProcessBotResponse(TextStreamEvent textEvent)
    {
        Send(textEvent.Text);
    }

    private void Connect()
    {
        client.LoginAsync(TokenType.Bearer, config.Value.Token)
            .GetAwaiter()
            .GetResult();

        client.StartAsync()
            .GetAwaiter()
            .GetResult();

        
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
        //client.SendMessage(Config.Channel, message);
    }

    public override void Init()
    {
        if (string.IsNullOrEmpty(config.Value.Token))
            return;

        if (config.Value.SubscribeToEvents)
        {
            eventListener.Subscribe<TextStreamEvent>(ProcessBotResponse);
        }

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