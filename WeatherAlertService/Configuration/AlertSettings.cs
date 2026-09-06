namespace WeatherAlertService.Configuration
{
    public class AlertSettings
    {
        public const string SectionName = "AlertSettings";
        public double HighTemperatureThreshold { get; set; } = 95;
        public double LowTemperatureThreshold { get; set; } = 20;
        public double HighWindSpeedThreshold { get; set; } = 30;
    }
}
