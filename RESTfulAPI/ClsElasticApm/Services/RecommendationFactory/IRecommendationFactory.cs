namespace WellingtonWeatherRecommendationApi.Services
{
    public interface IRecommendationFactory
    {
        string GetRecommendation(double temperature, string condition);
    }
}
