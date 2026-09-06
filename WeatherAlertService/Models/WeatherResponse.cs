using System.Text.Json.Serialization;

namespace WeatherAlertService.Models
{
    public sealed class WeatherResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("current")]
        public CurrentWeather? Current {  get; set; }
    }
    public class CurrentWeather
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }
        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }
    }
}