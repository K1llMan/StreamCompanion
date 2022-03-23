using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Classes.Models;
using CompanionPlugin.Classes.Services;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Logging;

using StreamEvents.Events;
using StreamEvents.Interfaces;

using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

using TwitchPlugin.Classes;

namespace TwitchPlugin.Services;

[StreamService("Twitch", "twitch.json")]
public class TwitchSourceService: CommandSourceService<TwitchSourceServiceConfig>
{
    #region Поля

    private IStreamEventsService eventListener;
    private ILogger<TwitchSourceService> log;
    
    private TwitchClient client;
    private TwitchAPI api;

    #endregion Поля

    #region Вспомогательные функции

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs args)
    {
        log.LogTrace($"{args.Channel}: {args.BotUsername}");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs args)
    {
        log.LogTrace(args.BotUsername);
    }

    private UserRole GetUserRole(ChatMessage message)
    {
        if (message.IsBroadcaster)
            return UserRole.Administrator;
        if (message.IsModerator)
            return UserRole.Moderator;

        return UserRole.User;
    }

    private void OnMessageReceived(object sender, OnMessageReceivedArgs args)
    {
        BotResponseMessage message = Received(new CommandReceivedArgs {
            Message = args.ChatMessage.Message,
            User = args.ChatMessage.DisplayName,
            Role = GetUserRole(args.ChatMessage)
        });

        switch (message)
        {
            case { Type: MessageType.Error }:
                Console.WriteLine(args.ChatMessage.Message);
                break;

            case { Type: MessageType.Success } when !string.IsNullOrEmpty(message.Text):
                Send(message.Text);
                break;
        };
    }

    private void ClientConnect()
    {
        // Token сгенерирован twitchtokengenerator
        ConnectionCredentials credentials = new(config.Value.Login, config.Value.Token);
        ClientOptions clientOptions = new()
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        WebSocketClient customClient = new(clientOptions);
        client = new TwitchClient(customClient);
        client.Initialize(credentials, config.Value.Channel);

        //client.OnLog += Client_OnLog;
        client.OnConnected += Client_OnConnected;
        client.OnJoinedChannel += ClientOnJoinedChannel;
        client.OnMessageReceived += OnMessageReceived;

        /*
        client.OnLog += Client_OnLog;
        client.OnJoinedChannel += Client_OnJoinedChannel;
        client.OnMessageReceived += OnMessageReceived;
        client.OnWhisperReceived += Client_OnWhisperReceived;
        client.OnNewSubscriber += Client_OnNewSubscriber;
        client.OnConnected += Client_OnConnected;
        */

        client.Connect();
    }

    private void APIConnect()
    {
        api = new TwitchAPI(settings: new ApiSettings {
            ClientId = config.Value.ClientId,
            AccessToken = config.Value.Token
        });
    }

    private async Task ProcessBotResponse(TextStreamEvent textEvent)
    {
        Send(textEvent.Text);
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public TwitchSourceService(IStreamEventsService events, IWritableOptions<TwitchSourceServiceConfig> serviceConfig, ILogger<TwitchSourceService> logger)
    {
        eventListener = events;
        log = logger;
        SetConfig(serviceConfig);
    }

    public void Send(string message)
    {
        client.SendMessage(config.Value.Channel, message);
    }

    public override void Init()
    {
        if (string.IsNullOrEmpty(config.Value.Token))
            return;

        if (config.Value.SubscribeToEvents)
        {
            eventListener.Subscribe<TextStreamEvent>(ProcessBotResponse);
        }

        ClientConnect();
        APIConnect();
    }

    public override void Dispose()
    {
        if (config.Value.SubscribeToEvents)
        {
            eventListener.Unsubscribe<TextStreamEvent>(ProcessBotResponse);
        }

        client?.Disconnect();

        base.Dispose();
    }

    #endregion Основные функции
}