namespace WeatherAlertService.Templates
{
    public static class EmailTemplate
    {
        public static string CreateWeatherAlertTemplate(
            double temperature,
            double windSpeed,
            string alertMessage)
        {
            return $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif;">
                    <h2>Weather Alert</h2>

                    <p>
                        <strong>Temperature:</strong>
                        {temperature:F1}°F
                    </p>

                    <p>
                        <strong>Wind Speed:</strong>
                        {windSpeed:F1} mph
                    </p>

                    <p>
                        <strong>Alert:</strong>
                        {alertMessage}
                    </p>

                    <hr />

                    <small>
                        WeatherAlertService Portfolio Project
                    </small>
                </body>
                </html>
                """;
        }
    }
}