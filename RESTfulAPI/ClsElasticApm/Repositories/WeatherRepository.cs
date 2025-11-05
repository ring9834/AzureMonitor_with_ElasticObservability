using Newtonsoft.Json;
using WellingtonWeatherRecommendationApi.Models;

namespace WellingtonWeatherRecommendationApi.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public WeatherRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? string.Empty;
            _baseUrl = configuration["OpenWeatherMap:BaseUrl"] ?? string.Empty;
        }

        public async Task<WeatherApiResponse> GetWeatherAsync(double latitude, double longitude)
        {
            var url = $"{_baseUrl}?lat={latitude}&lon={longitude}&units=metric&appid={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherApiResponse = JsonConvert.DeserializeObject<WeatherApiResponse>(content);
            return weatherApiResponse ?? throw new Exception("Failed to deserialize weather API response");
        }
    }
}
