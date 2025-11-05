using WellingtonWeatherRecommendationApi.Middleware;
using WellingtonWeatherRecommendationApi.Repositories;
using WellingtonWeatherRecommendationApi.Services;
using Elastic.Apm.NetCoreAll;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register HttpClient and dependencies
builder.Services.AddHttpClient<IWeatherRepository, WeatherRepository>();
builder.Services.AddSingleton<IRecommendationFactory, RecommendationFactory>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddSingleton<RateLimiterService>(); // Register RateLimiterService

var app = builder.Build();

// Enable Elastic APM
app.UseAllElasticApm(builder.Configuration);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp"); // Apply CORS policy before routing
app.UseMiddleware<RateLimitingMiddleware>(); // Add rate-limiting middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
