using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Web;

using DonationAlertsLib.Models.Common;
using DonationAlertsLib.Models.Sockets;

namespace DonationAlertsLib.Web;

#region События

/// <summary>
/// Событие получения сообщения
/// </summary>
public class ReceiveEventArgs
{
    public string Data { get; internal set; }
}

public delegate void RecieveEventHandler(object sender, ReceiveEventArgs e);

/// <summary>
/// Событие закрытия соединения
/// </summary>
public class CloseEventArgs
{
    public WebSocketCloseStatus? Status { get; internal set; }

    public string Description { get; internal set; }
}

public delegate void CloseEventHandler(object sender, CloseEventArgs e);

/// <summary>
/// Событие отправки данных
/// </summary>
public class SendEventArgs
{
    public string Data { get; internal set; }
}

public delegate void SendEventHandler(object sender, SendEventArgs e);

#endregion События

public class DASocketClient
{
    #region Поля

    private readonly Uri uri;
    private readonly StringBuilder data = new();
    private readonly int size = 4096;

    private readonly ClientWebSocket socketClient = new();

    private CancellationTokenSource cancellationTokenSource = new();
    private CancellationToken cancellation;

    #endregion Поля

    #region Свойства

    /// <summary>
    /// Подключён ли сокет
    /// </summary>
    public bool Connected => socketClient.State == WebSocketState.Open;

    #endregion Свойства

    #region События

    public event SendEventHandler OnSend;
    public event RecieveEventHandler OnReceive;
    public event CloseEventHandler OnClose;

    #endregion События

    #region Вспомогательные функции

    private string ReadSocketContent()
    {
        byte[] buffer = new byte[size];
        WebSocketReceiveResult result;

        do
        {
            result = socketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            data.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        return data.ToString();
    }

    #endregion Вспомогательные функции

    #region Вспомогательные функции

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    public async Task Send<TRequest>(TRequest sendingData)
    {
        if (socketClient.State != WebSocketState.Open)
            return;

        try
        {
            byte[] encoded = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendingData));
            await socketClient.SendAsync(new ArraySegment<byte>(encoded, 0, encoded.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Получение сообщения
    /// </summary>
    public TResponse Receive<TResponse>()
    {
        string content = ReadSocketContent();

        TResponse response = JsonSerializer.Deserialize<TResponse>(content, JsonSerializerSettings.GetSettings());
        data.Clear();

        return response;
    }

    private SocketResponse<TResponse> SendMessage<TRequest, TResponse>(SocketRequest<TRequest> message)
    {
        Send(message).GetAwaiter().GetResult();
        return Receive<SocketResponse<TResponse>>();
    }

    #endregion Вспомогательные функции

    #region Основные функции

    /// <summary>
    /// Получение сообщения
    /// </summary>
    public async Task BeginReceive()
    {
        if (socketClient.State != WebSocketState.Open)
            return;

        do
        {
            string content = ReadSocketContent();

            OnReceive?.Invoke(this, new ReceiveEventArgs {
                Data = HttpUtility.UrlDecode(content)
            });

            data.Clear();
        } while (!cancellation.IsCancellationRequested);

        await socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        OnClose?.Invoke(this, new CloseEventArgs {
            Status = socketClient.CloseStatus,
            Description = socketClient.CloseStatusDescription
        });
    }

    public string GetClientUuid(string socketToken)
    {
        return SendMessage<TokenRequest, ClientUuidResponse>(new SocketRequest<TokenRequest> {
            Id = 1,
            Params = new TokenRequest {
                Token = socketToken
            }
        }).Result.Client;
    }

    public SubscribeResponse[] Subscribe(ChannelInfo[] channels)
    {
        List<SubscribeResponse> subs = new();

        foreach (ChannelInfo channel in channels)
        {
            SocketResponse<SubscribeResponse> response = SendMessage<ChannelInfo, SubscribeResponse>(new SocketRequest<ChannelInfo> {
                Id = 2,
                Method = 1,
                Params = channel
            });

            subs.Add(response.Result);
        }

        return subs.ToArray();
    }

    public bool Connect()
    {
        socketClient.ConnectAsync(uri, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        cancellation = cancellationTokenSource.Token;

        return socketClient.State == WebSocketState.Open;
    }

    public void Disconnect()
    {
        cancellationTokenSource.Cancel(false);

        socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    public DASocketClient(string url)
    {
        uri = new Uri(url);
    }

    #endregion Основные функции
}