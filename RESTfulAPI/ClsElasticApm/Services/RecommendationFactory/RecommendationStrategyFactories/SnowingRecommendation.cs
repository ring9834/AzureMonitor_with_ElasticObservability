namespace WellingtonWeatherRecommendationApi.Services
{
    public class SnowingRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return temperature < 15 ? "Don't forget to bring a coat" : "Dress warmly for the snow";
        }
    }
}
