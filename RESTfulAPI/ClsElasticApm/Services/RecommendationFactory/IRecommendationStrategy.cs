namespace WellingtonWeatherRecommendationApi.Services
{
    public interface IRecommendationStrategy
    {
        string GetRecommendation(double temperature);
    }
}
