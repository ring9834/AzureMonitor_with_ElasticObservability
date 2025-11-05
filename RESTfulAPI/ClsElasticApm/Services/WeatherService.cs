using WellingtonWeatherRecommendationApi.Models.WellingtonWeatherApi.Models;
using WellingtonWeatherRecommendationApi.Repositories;

namespace WellingtonWeatherRecommendationApi.Services
{
    public class WeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IRecommendationFactory _recommendationService;

        public WeatherService(IWeatherRepository weatherRepository, IRecommendationFactory recommendationService)
        {
            _weatherRepository = weatherRepository;
            _recommendationService = recommendationService;
        }

        public async Task<WeatherResponse> GetWeatherForecastAsync(double latitude, double longitude)
        {
            var weatherData = await _weatherRepository.GetWeatherAsync(latitude, longitude);
            if (weatherData == null || weatherData.Weather == null || weatherData.Weather.Length == 0)
            {
                throw new Exception("Invalid weather data received from API");
            }

            // Map OpenWeatherMap condition to simplified condition
            string condition = MapWeatherCondition(weatherData?.Weather[0]?.Main ?? string.Empty);

            // Convert wind speed from m/s to km/h
            double windSpeedKph = weatherData?.Wind?.Speed * 3.6 ?? 0;

            // Get recommendation
            string recommendation = _recommendationService.GetRecommendation(weatherData?.Main?.Temp ?? 0, condition);

            return new WeatherResponse
            {
                TemperatureCelsius = weatherData?.Main?.Temp ?? 0,
                WindSpeedKph = windSpeedKph,
                Condition = condition,
                Recommendation = recommendation
            };
        }

        private string MapWeatherCondition(string apiCondition)
        {
            return apiCondition.ToLower() switch
            {
                "clear" => "Sunny",
                "rain" or "drizzle" => "Rainy",
                "snow" => "Snowing",
                "clouds" or "thunderstorm" or "tornado" => "Windy",
                _ => "Windy" // Default to Windy for unknown conditions
            };
        }
    }
}
