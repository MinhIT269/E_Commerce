using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace E_Commerce.UI.Helpers;

public class ApiRequestHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _baseUrl;

    public ApiRequestHelper(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _baseUrl = config["ApiSettings:BaseUrl"]!;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string relativeUrl, object? data = null)
    {
        try
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(method, $"{_baseUrl}{relativeUrl}");

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>()
                : default;
        }
        catch
        {
            return default;
        }
    }

    public Task<T?> SendGetRequestAsync<T>(string url) => SendAsync<T>(HttpMethod.Get, url);
    public Task<T?> SendPostRequestAsync<T>(string url, object data) => SendAsync<T>(HttpMethod.Post, url, data);
    public Task<T?> SendPutRequestAsync<T>(string url, object data) => SendAsync<T>(HttpMethod.Put, url, data);
    public Task<T?> SendDeleteRequestAsync<T>(string url, object data) => SendAsync<T>(HttpMethod.Delete, url, data);
}
