using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using WeatherAlertService.Models;

namespace WeatherAlertService.Data;

public sealed class WeatherRepository
{
    private readonly string _connectionString;
    private readonly ILogger<WeatherRepository> _logger;

    public WeatherRepository(
        IConfiguration configuration,
        ILogger<WeatherRepository> logger)
    {
        _connectionString =
            configuration.GetConnectionString("WeatherDatabase")
            ?? throw new InvalidOperationException(
                "WeatherDatabase connection string is missing.");

        _logger = logger;
    }

    public async Task SaveAsync(
        WeatherResponse weather,
        CancellationToken cancellationToken = default)
    {
        if (weather.Current is null)
        {
            throw new ArgumentException(
                "Current weather data is missing.",
                nameof(weather));
        }

        const string sql = """
            INSERT INTO dbo.WeatherData
            (
                Latitude,
                Longitude,
                TemperatureF,
                WindSpeedMph,
                WeatherTime
            )
            VALUES
            (
                @Latitude,
                @Longitude,
                @TemperatureF,
                @WindSpeedMph,
                @WeatherTime
            );
            """;

        DateTime? weatherTime = ParseWeatherTime(
            weather.Current.Time);

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(sql, connection);

        AddDecimalParameter(
            command,
            "@Latitude",
            weather.Latitude,
            precision: 9,
            scale: 6);

        AddDecimalParameter(
            command,
            "@Longitude",
            weather.Longitude,
            precision: 9,
            scale: 6);

        AddDecimalParameter(
            command,
            "@TemperatureF",
            weather.Current.Temperature,
            precision: 6,
            scale: 2);

        AddDecimalParameter(
            command,
            "@WindSpeedMph",
            weather.Current.WindSpeed,
            precision: 6,
            scale: 2);

        var weatherTimeParameter =
            command.Parameters.Add(
                "@WeatherTime",
                SqlDbType.DateTime2);

        weatherTimeParameter.Value =
            weatherTime.HasValue
                ? weatherTime.Value
                : DBNull.Value;

        try
        {
            await connection.OpenAsync(
                cancellationToken);

            await command.ExecuteNonQueryAsync(
                cancellationToken);

            _logger.LogInformation(
                "Weather data saved successfully for {Latitude}, {Longitude}",
                weather.Latitude,
                weather.Longitude);
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                "Failed to save weather data to SQL Server.");

            throw;
        }
    }

    private static DateTime? ParseWeatherTime(
        string? weatherTime)
    {
        if (string.IsNullOrWhiteSpace(weatherTime))
        {
            return null;
        }

        if (DateTime.TryParse(
            weatherTime,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedTime))
        {
            return parsedTime;
        }

        return null;
    }

    private static void AddDecimalParameter(
        SqlCommand command,
        string parameterName,
        double value,
        byte precision,
        byte scale)
    {
        var parameter =
            command.Parameters.Add(
                parameterName,
                SqlDbType.Decimal);

        parameter.Precision = precision;
        parameter.Scale = scale;
        parameter.Value = Convert.ToDecimal(value);
    }
}