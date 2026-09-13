using Serilog;
using WeatherAlertService.Api;
using WeatherAlertService.Configuration;
using WeatherAlertService.Data;
using WeatherAlertService.Models;
using WeatherAlertService.Services;
using WeatherAlertService.Templates;


// ----------------------------------------------------
// Serilog Configuration
// ----------------------------------------------------

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/weather-log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} " +
            "[{Level:u3}] " +
            "{SourceContext} - " +
            "{Message:lj}{NewLine}{Exception}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();


// ----------------------------------------------------
// Services
// ----------------------------------------------------

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Open-Meteo API client
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress =
        new Uri("https://api.open-meteo.com/");
});


// Application services
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<WeatherRepository>();


// ----------------------------------------------------
// Configuration
// ----------------------------------------------------

builder.Services.Configure<AlertSettings>(
    builder.Configuration.GetSection(
        AlertSettings.SectionName));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection(
        SmtpSettings.SectionName));


// ----------------------------------------------------
// Build Application
// ----------------------------------------------------

var app = builder.Build();


// ----------------------------------------------------
// Middleware
// ----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapRazorPages();


// ====================================================
// WEATHER API ENDPOINT
// ====================================================
//
// Retrieves current weather from Open-Meteo.
//
// Example:
// GET /weather?latitude=38.25&longitude=-85.76
//

app.MapGet(
    "/weather",
    async (
        double latitude,
        double longitude,
        WeatherService weatherService,
        CancellationToken cancellationToken) =>
    {
        var weather =
            await weatherService.GetWeatherAsync(
                latitude,
                longitude,
                cancellationToken);

        if (weather is null)
        {
            return Results.Problem(
                "Unable to retrieve weather data.");
        }

        return Results.Ok(weather);
    })
    .WithName("GetWeather");


// ====================================================
// WEATHER ALERT CHECK ENDPOINT
// ====================================================
//
// Retrieves live weather and evaluates it against
// configured alert thresholds.
//
// Example:
// GET /alerts/check?latitude=38.25&longitude=-85.76
//

app.MapGet(
    "/alerts/check",
    async (
        double latitude,
        double longitude,
        WeatherService weatherService,
        AlertService alertService,
        CancellationToken cancellationToken) =>
    {
        var weather =
            await weatherService.GetWeatherAsync(
                latitude,
                longitude,
                cancellationToken);

        if (weather is null)
        {
            return Results.Problem(
                "Unable to retrieve weather data.");
        }

        var alertResult =
            alertService.Evaluate(weather);

        return Results.Ok(
            new
            {
                Weather = weather,
                Alert = alertResult
            });
    })
    .WithName("CheckWeatherAlert");


// ====================================================
// SAVE WEATHER TO SQL SERVER
// ====================================================
//
// Retrieves live weather from Open-Meteo and saves
// the result to SQL Server.
//
// Example:
// POST /weather/save?latitude=38.25&longitude=-85.76
//

app.MapPost(
    "/weather/save",
    async (
        double latitude,
        double longitude,
        WeatherService weatherService,
        WeatherRepository weatherRepository,
        CancellationToken cancellationToken) =>
    {
        var weather =
            await weatherService.GetWeatherAsync(
                latitude,
                longitude,
                cancellationToken);

        if (weather is null)
        {
            return Results.Problem(
                "Unable to retrieve weather data.");
        }

        await weatherRepository.SaveAsync(
            weather,
            cancellationToken);

        return Results.Ok(
            new
            {
                Message =
                    "Weather data saved successfully.",
                Weather = weather
            });
    })
    .WithName("SaveWeather");


// ====================================================
// DEMO WEATHER ALERT ENDPOINT
// ====================================================
//
// One-click Swagger demo.
//
// Uses static weather values so the application
// reliably triggers an alert and sends an email
// through Mailtrap.
//
// No request parameters are required.
//

app.MapPost(
    "/alerts/test",
    async (
        AlertService alertService,
        EmailService emailService,
        CancellationToken cancellationToken) =>
    {
        // Static demo values
        const double temperature = 105.0;
        const double windSpeed = 10.0;

        // Mailtrap Sandbox captures this email.
        // It is not delivered to this address.
        const string recipient = "demo@example.com";


        var weather = new WeatherResponse
        {
            Latitude = 38.25,
            Longitude = -85.76,

            Current = new CurrentWeather
            {
                Time =
                    DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm"),

                Temperature = temperature,
                WindSpeed = windSpeed
            }
        };


        // Apply the same business rules used
        // by live weather data.
        var alertResult =
            alertService.Evaluate(weather);


        // If thresholds are not exceeded,
        // no email is sent.
        if (!alertResult.IsTriggered)
        {
            return Results.Ok(
                new
                {
                    AlertTriggered = false,
                    EmailSent = false,

                    Temperature = temperature,
                    WindSpeed = windSpeed,

                    alertResult.Message
                });
        }


        // Generate HTML email body
        var htmlBody =
            EmailTemplate.CreateWeatherAlertTemplate(
                temperature,
                windSpeed,
                alertResult.Message);


        // Send email through SMTP / Mailtrap
        await emailService.SendAsync(
            recipient,
            "Weather Alert - Demo",
            htmlBody,
            cancellationToken);


        return Results.Ok(
            new
            {
                AlertTriggered = true,
                EmailSent = true,

                Temperature = temperature,
                WindSpeed = windSpeed,

                Recipient = recipient,

                alertResult.Message
            });
    })
    .WithName("TriggerDemoWeatherAlert");


// ----------------------------------------------------
// Run Application
// ----------------------------------------------------

app.Run();