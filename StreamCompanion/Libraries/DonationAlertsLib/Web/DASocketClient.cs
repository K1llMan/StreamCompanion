using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Web;

using DonationAlertsLib.Models.Sockets;

namespace DonationAlertsLib.Web;

public class DASocketClient
{
    #region Поля

    private readonly Uri uri;
    private readonly StringBuilder data = new();
    private readonly int size = 4096;

    private readonly ClientWebSocket socketClient = new();

    #endregion Поля

    #region Свойства

    /// <summary>
    /// Подключён ли сокет
    /// </summary>
    public bool Connected => socketClient.State == WebSocketState.Open;

    #endregion Свойства


    #region События

    /// <summary>
    /// Событие получения сообщения
    /// </summary>
    public class ReceiveEventArgs
    {
        public string Data { get; internal set; }
    }

    public delegate void RecieveEventHandler(object sender, ReceiveEventArgs e);
    public event RecieveEventHandler OnReceive;

    /// <summary>
    /// Событие закрытия соединения
    /// </summary>
    public class CloseEventArgs
    {
        public WebSocketCloseStatus? Status { get; internal set; }

        public string Description { get; internal set; }
    }

    public delegate void CloseEventHandler(object sender, CloseEventArgs e);
    public event CloseEventHandler OnClose;

    /// <summary>
    /// Событие отправки данных
    /// </summary>
    public class SendEventArgs
    {
        public string Data { get; internal set; }
    }

    public delegate void SendEventHandler(object sender, SendEventArgs e);
    public event SendEventHandler OnSend;

    #endregion События

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
    public async Task<TResponse> Receive<TResponse>()
    {
        byte[] buffer = new byte[size];
        WebSocketReceiveResult result;

        do
        {
            result = await socketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            data.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        TResponse response = JsonSerializer.Deserialize<TResponse>(data.ToString());
        data.Clear();

        return response;
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

        byte[] buffer = new byte[size];
        WebSocketReceiveResult result;
        do
        {
            result = await socketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            data.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

            // Вызов события 
            if (result.EndOfMessage)
            {
                OnReceive?.Invoke(this, new ReceiveEventArgs {
                    Data = HttpUtility.UrlDecode(data.ToString())
                });

                data.Clear();
            }
        } while (!result.CloseStatus.HasValue);

        await socketClient.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        OnClose?.Invoke(this, new CloseEventArgs {
            Status = socketClient.CloseStatus,
            Description = socketClient.CloseStatusDescription
        });
    }

    public SocketResponse<TResponse> SendMessage<TRequest, TResponse>(SocketRequest<TRequest> message)
    {
        Send(message).GetAwaiter().GetResult();
        return Receive<SocketResponse<TResponse>>().GetAwaiter().GetResult();
    }

    public bool Connect()
    {
        socketClient.ConnectAsync(uri, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        return socketClient.State == WebSocketState.Open;
    }

    public void Disconnect()
    {
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