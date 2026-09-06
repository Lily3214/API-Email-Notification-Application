namespace WeatherAlertService.Models;

public sealed record AlertResult(
    bool IsTriggered,
    string Message);