using WeatherAlertService.Api;
using WeatherAlertService.Models;

namespace WeatherAlertService.Services;

public sealed class WeatherService
{
    private readonly WeatherApiClient _weatherApiClient;

    public WeatherService(WeatherApiClient weatherApiClient)
    {
        _weatherApiClient = weatherApiClient;
    }

    public Task<WeatherResponse?> GetWeatherAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        return _weatherApiClient.GetWeatherAsync(
            latitude,
            longitude,
            cancellationToken);
    }
}