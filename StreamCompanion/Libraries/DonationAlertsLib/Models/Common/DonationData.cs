using System.Text.Json.Serialization;

namespace DonationAlertsLib.Models.Common;

public class DonationData
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    [JsonPropertyName("recipient_name")]
    public string RecipientName { get; set; }
    public string Message { get; set; }
    [JsonPropertyName("message_type")]
    public string MessageType { get; set; }
    [JsonPropertyName("payin_system")]
    public string PayingSystem { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    [JsonPropertyName("is_shown")]
    public int IsShown { get; set; }
    [JsonPropertyName("amount_in_user_currency")]
    public decimal AmountInUserCurrency { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("shown_at")]
    public DateTime ShownAt { get; set; }
    public string Reason { get; set; }
}