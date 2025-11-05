namespace WellingtonWeatherRecommendationApi.Services
{
    public class RecommendationFactory: IRecommendationFactory
    {
        private readonly Dictionary<string, IRecommendationStrategy> _strategies;

        public RecommendationFactory()
        {
            _strategies = new Dictionary<string, IRecommendationStrategy>
            {
                { "Sunny", new SunnyRecommendation() },
                { "Rainy", new RainyRecommendation() },
                { "Snowing", new SnowingRecommendation() },
                { "Windy", new WindyRecommendation() }
            };
        }

        public string GetRecommendation(double temperature, string condition)
        {
            if (_strategies.TryGetValue(condition, out var strategy))
            {
                return strategy.GetRecommendation(temperature);
            }
            return "Dress appropriately for the weather";
        }
    }
}
