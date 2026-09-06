using System.Net.Http.Json;
using WeatherAlertService.Models;

namespace WeatherAlertService.Api;
    public class WeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherApiClient> _logger;
        public WeatherApiClient(
            HttpClient httpClient,
            ILogger<WeatherApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<WeatherResponse?> GetWeatherAsync(double latitude, double longitude,
            CancellationToken cancellationToken = default)
        {
            string url =
                $"v1/forecast" +
                $"?latitude={latitude}" +
                $"&longitude={longitude}" +
                $"&current=temperature_2m,wind_speed_10m" +
                $"&temperature_unit=fahrenheit" +
                $"&wind_speed_unit=mph" +
                $"&timezone=auto";

            try
            {
                _logger.LogInformation(
                    "Requesting weather for {Latitude}, {Longtitude}",
                    latitude,
                    longitude);
                return await _httpClient.GetFromJsonAsync<WeatherResponse>(
                    url,
                    cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve weather data.");
                return null;
            }
        }
    }
