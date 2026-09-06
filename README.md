# API-Email-Notification-Application
Automated a previously manual monitoring/notification process.

## Project Overview

| Category | Description |
|---|---|
| **Project** | Automated ERP Data Integration & Email Notification System |
| **Problem** | Needed an automated solution to retrieve and process scrap data from Plex ERP and integrate it with SAP ERP. |
| **My Role** | Designed and developed the API-based data integration and automated notification workflow. |
| **Technology** | C#, .NET, REST API, JSON, SMTP |
| **Architecture** | Plex API → Retrieve Scrap Data → Save as JSON → Map Data Attributes → Apply Validation & Business Logic → Post to SAP ZERP → Send Email Notification |
| **Challenges** | API authentication, response parsing, data mapping, validation, error handling, and notification logic |
| **Implementation** | API client, JSON processing, mapping logic, validation/business rules, SAP integration, SMTP notifications, and logging/error handling |
| **Result** | Automated daily scrap-data processing and provided business users with a scheduled email notification every morning at 9:00 AM. |
| **Skills** | C#, .NET, REST API Integration, ERP Integration, Data Transformation, Automation, Troubleshooting |

Each Class have one responsibility:
| File | Responsibility |
|---|---|
| **Api/WeatherApiClient.cs** | Communicates with the Open-Meteo API using HttpClient and deserializes the API response |
| **Models/WeatherResponse.cs** | Represents the weather data returned by the Open-Meteo API |
| **Models/AlertResult.cs** | Represents the result of evaluating weather conditions, including whether an alert was triggered and the alert message |
| **Services/WeatherService.cs** | Coordinates retrieving weather information through WeatherApiClient |
| **Services/AlertService.cs** | Applies business rules to determine whether temperature or wind conditions require an alert |
| **Services/EmailService.cs** | Sends email notifications through SMTP/Mailtrap |
| **Templates/EmailTemplate.cs** | Builds the HTML content used for weather-alert emails |
| **Configuration/AlertSettings.cs** | Defines configurable temperature and wind-speed alert thresholds |
| **Configuration/SmtpSettings.cs** | Defines SMTP configuration such as host, port, username, sender address, and credentials |
| **appsettings.json** | Stores non-secret application configuration such as alert thresholds and SMTP settings |
| **Properties/launchSettings.json** | Defines local development launch settings, such as the application URL and Swagger launch page |
| **Program.cs** | Configures dependency injection, middleware, Swagger, application services, and API endpoints |

<img width="875" height="1016" alt="image" src="https://github.com/user-attachments/assets/e245266d-c934-45c5-98a8-b973574bf3b1" />

<img width="1233" height="1291" alt="image" src="https://github.com/user-attachments/assets/26425129-4efa-49af-b096-801c3230c306" />

<img width="1231" height="1292" alt="image" src="https://github.com/user-attachments/assets/8ffe472b-a7bb-4fcc-88e3-db240c2effa9" />

<img width="980" height="657" alt="image" src="https://github.com/user-attachments/assets/f4f9fa04-a580-4d7a-8e74-dfe8728b2ad6" />
