using Microsoft.Extensions.Options;
using WeatherAlertService.Configuration;
using WeatherAlertService.Models;

namespace WeatherAlertService.Services;

public sealed class AlertService
{
    private readonly AlertSettings _settings;

    public AlertService(IOptions<AlertSettings> options)
    {
        _settings = options.Value;
    }

    public AlertResult Evaluate(WeatherResponse weather)
    {
        if (weather.Current is null)
        {
            return new AlertResult(
                false,
                "Current weather data is unavailable.");
        }

        var current = weather.Current;

        if (current.Temperature >= _settings.HighTemperatureThreshold)
        {
            return new AlertResult(
                true,
                $"High temperature alert: {current.Temperature:F1}°F");
        }

        if (current.Temperature <= _settings.LowTemperatureThreshold)
        {
            return new AlertResult(
                true,
                $"Low temperature alert: {current.Temperature:F1}°F");
        }

        if (current.WindSpeed >= _settings.HighWindSpeedThreshold)
        {
            return new AlertResult(
                true,
                $"High wind alert: {current.WindSpeed:F1} mph");
        }

        return new AlertResult(
            false,
            "No weather alert.");
    }
}