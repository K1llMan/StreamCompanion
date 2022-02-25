using System.Text;
using System.Text.Json;

namespace DonationAlertsLib.Web.Requests;

public class BaseApiRequest
{
    #region Поля

    private readonly string rootUrl = "https://www.donationalerts.com/";
    private HttpClient client;

    #endregion Поля

    #region Вспомогательные функции

    protected List<KeyValuePair<string, string>>? GetDefaultHeaders(string token)
    {
        return new List<KeyValuePair<string, string>> {
            new ("Content-Type", "application/json"),
            new ("Authorization", $"Bearer {token}")
        };
    }

    #endregion Вспомогательные функции

    #region Вспомогательные функции

    protected TResponse GetResponse<TRequest, TResponse>(string url, HttpMethod method, Dictionary<string, string>? query = null, 
        List<KeyValuePair<string, string>>? headers = null, TRequest? body = null) where TRequest : class
    {
        HttpRequestMessage request = new(method, url);
        if (query != null)
            foreach (KeyValuePair<string, string> pair in query)
                request.Options.Set(new HttpRequestOptionsKey<string>(pair.Key), pair.Value);

        if (headers != null)
            foreach (KeyValuePair<string, string> pair in headers)
                request.Headers.Add(pair.Key, pair.Value);

        if (body != null)
            request.Content = new StringContent(JsonSerializer.Serialize(body, JsonSerializerSettings.GetSettings()), Encoding.UTF8);

        string response = client.Send(new HttpRequestMessage(method, url))
            .Content
            .ReadAsStringAsync()
            .GetAwaiter()
            .GetResult();

        return JsonSerializer.Deserialize<TResponse>(response, JsonSerializerSettings.GetSettings());
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public BaseApiRequest()
    {
        client = new HttpClient {
            BaseAddress = new Uri(rootUrl)
        };
    }

    #endregion Основные функции
}