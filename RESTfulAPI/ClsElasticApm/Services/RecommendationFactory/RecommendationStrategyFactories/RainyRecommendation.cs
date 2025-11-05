namespace WellingtonWeatherRecommendationApi.Services
{
    public class RainyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return temperature < 15 ? "Don't forget to bring a coat" : "Don’t forget the umbrella";
        }
    }
}
