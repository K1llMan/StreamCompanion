namespace DonationAlertsLib.Models.Sockets;

public class SocketResponse<T>
{
    public int Id { get; set; }
    public T Result { get; set; }
}