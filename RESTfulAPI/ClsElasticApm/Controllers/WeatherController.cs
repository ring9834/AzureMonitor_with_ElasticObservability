using Microsoft.AspNetCore.Mvc;
using WellingtonWeatherRecommendationApi.Models.WellingtonWeatherApi.Models;
using WellingtonWeatherRecommendationApi.Services;

namespace WellingtonWeatherRecommendationApi.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<ActionResult<WeatherResponse>> GetWeather(double latitude, double longitude)
        {
            var response = await _weatherService.GetWeatherForecastAsync(latitude, longitude);
            return Ok(response);
        }
    }
}
