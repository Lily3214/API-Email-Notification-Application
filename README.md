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
| **WeatherApiClient.cs** | Communicate with Open-Meteo using HttpClient |
| **WeatherResponse.cs** | Represent/deserializes API JSON |
| **WeatherService.cs** | Coordinate obtaining weather information |
| **AlertService.cs** | Decide whether weather conditions require an alert |
| **EmailService.cs** | Build and send email notifications |
| **AlertSettings.cs** | Store configurable thresholds |
| **appsettings.json** | Configuration values |
| **Program.cs** | Dependency injection and application startup |

<img width="875" height="1016" alt="image" src="https://github.com/user-attachments/assets/e245266d-c934-45c5-98a8-b973574bf3b1" />

<img width="1233" height="1291" alt="image" src="https://github.com/user-attachments/assets/26425129-4efa-49af-b096-801c3230c306" />

<img width="1231" height="1292" alt="image" src="https://github.com/user-attachments/assets/8ffe472b-a7bb-4fcc-88e3-db240c2effa9" />

<img width="1211" height="691" alt="image" src="https://github.com/user-attachments/assets/19ec8ad4-1aaf-471f-94d1-aaf3e00d086b" />
