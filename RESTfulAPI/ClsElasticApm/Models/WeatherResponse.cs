namespace WellingtonWeatherRecommendationApi.Models
{
    namespace WellingtonWeatherApi.Models
    {
        public class WeatherResponse
        {
            public double TemperatureCelsius { get; set; }
            public double WindSpeedKph { get; set; }
            public string? Condition { get; set; }
            public string? Recommendation { get; set; }
        }
    }
}
