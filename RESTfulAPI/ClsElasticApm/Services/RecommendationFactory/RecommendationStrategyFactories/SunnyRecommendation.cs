namespace WellingtonWeatherRecommendationApi.Services
{
    public class SunnyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return temperature > 25 ? "It’s a great day for a swim" : "Don't forget to bring a hat";
        }
    }
}
