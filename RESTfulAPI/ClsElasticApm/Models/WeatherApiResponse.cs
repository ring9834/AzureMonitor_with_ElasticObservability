namespace WellingtonWeatherRecommendationApi.Models
{
    public class WeatherApiResponse
    {
        public Main? Main { get; set; }
        public Wind? Wind { get; set; }
        public Weather[]? Weather { get; set; }
    }

    public class Main
    {
        public double Temp { get; set; }
        public int Humidity { get; set; }
    }

    public class Wind
    {
        public double Speed { get; set; }
    }

    public class Weather
    {
        public string? Main { get; set; }
    }
}
