using WellingtonWeatherRecommendationApi.Models;

namespace WellingtonWeatherRecommendationApi.Repositories
{
    public interface IWeatherRepository
    {
        Task<WeatherApiResponse> GetWeatherAsync(double latitude, double longitude);
    }
}
