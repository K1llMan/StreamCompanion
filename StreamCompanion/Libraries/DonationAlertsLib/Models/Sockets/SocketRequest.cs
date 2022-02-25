namespace DonationAlertsLib.Models.Sockets;

public class SocketRequest<T>
{
    public int Id { get; set; }
    public int Method { get; set; }
    public T Params { get; set; }
}