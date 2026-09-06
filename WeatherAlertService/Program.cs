using WeatherAlertService.Api;
using WeatherAlertService.Configuration;
using WeatherAlertService.Services;
using WeatherAlertService.Models;
using WeatherAlertService.Templates;

var builder = WebApplication.CreateBuilder(args);


// -------------------------------
// Services
// -------------------------------

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress =
        new Uri("https://api.open-meteo.com/");
});

builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<EmailService>();


// -------------------------------
// Configuration
// -------------------------------

builder.Services.Configure<AlertSettings>(
    builder.Configuration.GetSection(
        AlertSettings.SectionName));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection(
        SmtpSettings.SectionName));


// -------------------------------
// Build Application
// -------------------------------

var app = builder.Build();


// -------------------------------
// Middleware
// -------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


// -------------------------------
// Weather Endpoint
// -------------------------------

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

        return weather is null
            ? Results.Problem(
                "Unable to retrieve weather.")
            : Results.Ok(weather);
    })
    .WithName("GetWeather");


// -------------------------------
// Alert Endpoint
// -------------------------------

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
                "Unable to retrieve weather.");
        }

        var result =
            alertService.Evaluate(weather);

        return Results.Ok(result);
    })
    .WithName("CheckWeatherAlert");

app.MapPost(
    "alert/test",
    async (
        double temperature,
        double windSpeed,
        string recipient,
        AlertService alertService,
        EmailService emailService,
        CancellationToken cancellationToken) =>
    {
        // Create fake weather data for testing
        var weather = new WeatherResponse
        {
            Latitude = 38.25,
            Longitude = -85.76,

            Current = new CurrentWeather
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Temperature = temperature,
                WindSpeed = windSpeed,
            }
        };

        // Same business rules used with real weather data.
        var alertResult = alertService.Evaluate(weather);

        // No alert = don't send email
        if (!alertResult.IsTriggered)
        {
            return Results.Ok(new
            {
                AlertTriggered = false,
                EmailSent = false,
                alertResult.Message
            });
        }

        // build HTML email
        var htmlBody =
        EmailTemplate.CreateWeatherAlertTemplate(
            temperature,
            windSpeed,
            alertResult.Message);

        // Send through Mailtrap
        await emailService.SendAsync(
            recipient,
            "Weather Alert",
            htmlBody,
            cancellationToken);

        return Results.Ok(new
        {
            AlertTriggered = true,
            EmailSent = true,
            alertResult.Message
        });
    })
    .WithName("TestWeatherAlert");
/*
// -------------------------------
// Email Test Endpoint
// -------------------------------

app.MapPost(
    "/email/test",
    async (
        string recipient,
        EmailService emailService,
        CancellationToken cancellationToken) =>
    {
        const string subject =
            "WeatherAlertService Test";

        const string body = """
            <html>
            <body>
                <h2>Weather Alert Service</h2>

                <p>
                    SMTP integration is working.
                </p>

                <hr />

                <small>
                    WeatherAlertService Portfolio Project
                </small>
            </body>
            </html>
            """;

        await emailService.SendAsync(
            recipient,
            subject,
            body,
            cancellationToken);

        return Results.Ok(new
        {
            Message = "Test email sent successfully."
        });
    })
    .WithName("SendTestEmail");
*/

app.Run();