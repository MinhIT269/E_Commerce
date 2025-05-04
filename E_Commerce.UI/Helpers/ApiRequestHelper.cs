using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace E_Commerce.UI.Helpers
{
    public class ApiRequestHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public ApiRequestHelper(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> SendGetRequestAsync<T>(string relativeUrl)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
                var client = _httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_baseUrl}{relativeUrl}")
                };

                var httpResponse = await client.SendAsync(httpMessage);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return default;
                }

                return await httpResponse.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T?> SendPostRequestAsync<T>(string relativeUrl, object data)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
                var client = _httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_baseUrl}{relativeUrl}"),
                    Content = content
                };

                var httpResponse = await client.SendAsync(httpMessage);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return default;
                }

                return await httpResponse.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception)
            {
                return default;
            }
        }
   
        public async Task<T?> SendDeleteRequestAsync<T>(string relativeUrl, object data)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
                var client = _httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"{_baseUrl}{relativeUrl}")
                };

                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return default;

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> SendPutRequestAsync<T>(string relativeUrl, object data)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
                var client = _httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"{_baseUrl}{relativeUrl}"),
                    Content = content
                };

                var httpResponse = await client.SendAsync(httpMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return await httpResponse.Content.ReadFromJsonAsync<T>();
                }

                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
