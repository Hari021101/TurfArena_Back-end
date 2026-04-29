# Turf Arena API

The backend REST API for the Turf Arena application, built with ASP.NET Core 8 and Entity Framework Core. This API serves as the central data and authentication hub for the Turf Arena React Native mobile application.

## 🚀 Technologies

*   **Framework**: ASP.NET Core 8 Web API
*   **ORM**: Entity Framework Core 8
*   **Database**: SQL Server (LocalDB for development)
*   **Authentication**: JWT (JSON Web Tokens) with BCrypt Password Hashing
*   **Documentation**: Swagger / OpenAPI

## 📁 Project Structure

```text
TurfArena/
├── Controllers/         # API Endpoint Handlers
│   ├── AuthController.cs    # Registration, Login, Profile updates
│   ├── TurfsController.cs   # Turf CRUD operations
│   ├── SlotsController.cs   # Availability and time slot generation
│   ├── BookingsController.cs# Booking creation and management
│   └── ReviewsController.cs # Ratings and review submissions
├── Data/                # Database Context & Configuration
│   └── AppDbContext.cs      # EF Core DbContext with model configurations
├── DTOs/                # Data Transfer Objects
│   ├── Auth/                # Registration/Login DTOs
│   ├── Booking/             # Booking request/response DTOs
│   ├── Review/              # Review DTOs
│   └── Turf/                # Turf listing DTOs
├── Models/              # Database Entities (Domain Models)
│   ├── AppUser.cs           # User profiles
│   ├── Turf.cs              # Turf listings
│   ├── Slot.cs              # Hourly time slots
│   ├── Booking.cs           # Reservations
│   ├── Review.cs            # User feedback
│   └── Payment.cs           # Transaction details
├── Services/            # Core Business Logic
│   ├── AuthService.cs           # JWT Generation and User validation
│   └── SlotGeneratorService.cs  # Dynamic hourly slot creation
├── Program.cs           # Application bootstrapping, Middleware, DI Configuration
└── appsettings.json     # Configuration (Connection Strings, JWT keys)
```

## 🛠️ Setup & Installation

### 1. Prerequisites
*   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   SQL Server Express LocalDB (usually installed automatically with Visual Studio)

### 2. Configure Settings
Ensure your `appsettings.json` has the correct `DefaultConnection` string and a strong `Jwt:Key`.

### 3. Apply Database Migrations
Run the following commands using the .NET CLI in your terminal to generate the SQL Server database schema:
```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the API
Start the development server using Visual Studio or the terminal:
```powershell
dotnet run
```

## 📚 API Documentation
Once the application is running, navigate to the Swagger UI in your browser to view and test all endpoints interactively:
`https://localhost:7085/swagger` (Port may vary based on `launchSettings.json`).
