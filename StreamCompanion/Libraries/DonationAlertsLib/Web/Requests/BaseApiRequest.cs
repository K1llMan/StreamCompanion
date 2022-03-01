using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DonationAlertsLib.Web.Requests;

public class BaseApiRequest
{
    #region Поля

    private readonly string rootUrl = "https://www.donationalerts.com/";
    private HttpClient client;

    #endregion Поля

    #region Вспомогательные функции

    protected List<KeyValuePair<string, string>> GetDefaultHeaders(string token = "")
    {
        List<KeyValuePair<string, string>> headers = new();

        if (!string.IsNullOrEmpty(token))
            headers.Add(new("Authorization", $"Bearer {token}"));

        return headers;
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
            request.Content = JsonContent.Create(body, new MediaTypeHeaderValue("application/json"), JsonSerializerSettings.GetSettings());

        HttpResponseMessage message = client.Send(request);

        string response = message
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