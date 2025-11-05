namespace WellingtonWeatherRecommendationApi.Services
{
    public class WindyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return "Wear a windbreaker";
        }
    }
}
