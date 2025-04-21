namespace E_Commerce.UI.Helpers
{
    public class ApiRequestHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public ApiRequestHelper(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
        }

        public async Task<T?> SendGetRequestAsync<T>(string relativeUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
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
    }
}
