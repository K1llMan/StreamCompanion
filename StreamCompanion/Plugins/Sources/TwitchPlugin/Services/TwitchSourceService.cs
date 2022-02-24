using System.ComponentModel;

using CompanionPlugin.Classes;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;

using Microsoft.Extensions.Logging;

using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

using TwitchPlugin.Classes;

namespace TwitchPlugin.Services;

[Description("Twitch")]
public class TwitchSourceService: CommandSourceService
{
    #region Поля

    private IWritableOptions<TwitchSourceServiceConfig> config;
    private ILogger<TwitchSourceService> log;
    private TwitchClient client;

    #endregion Поля

    #region Свойства

    /// <summary>
    /// Конфигурация
    /// </summary>
    internal TwitchSourceServiceConfig Config => config.Value;

    /// <summary>
    /// API
    /// </summary>
    public TwitchAPI API { get; private set; }

    #endregion Свойства

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
        BotMessage message = Received(new CommandReceivedArgs {
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
        ConnectionCredentials credentials = new(Config.Login, Config.Token);
        ClientOptions clientOptions = new()
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        WebSocketClient customClient = new(clientOptions);
        client = new TwitchClient(customClient);
        client.Initialize(credentials, Config.Channel);

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
        API = new TwitchAPI(settings: new ApiSettings {
            ClientId = Config.ClientId,
            AccessToken = Config.Token
        });
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public TwitchSourceService(IWritableOptions<TwitchSourceServiceConfig> serviceConfig, ILogger<TwitchSourceService> logger)
    {
        config = serviceConfig;
        log = logger;
    }

    public void Send(string message)
    {
        client.SendMessage(Config.Channel, message);
    }

    public override void Init()
    {
        if (string.IsNullOrEmpty(config.Value.Token))
            return;

        ClientConnect();
        APIConnect();
    }

    #endregion Основные функции
}