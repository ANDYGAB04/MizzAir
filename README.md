# MizzAir - Aviation Management System

**Authors:** Suhan Mihai-Andrei, Mogosanu Florin, Papura Corneliu-Octavian

## Overview

MizzAir is a comprehensive web application designed for aviation companies to streamline and automate flight and customer management processes. The system efficiently handles flight data, passenger information, bookings, baggage management, and staff administration—providing a complete solution for modern aviation operations.

## Features

- **Flight Management** - Create, update, and manage flight schedules with real-time information
- **Passenger Bookings** - Intuitive booking system with seat selection and baggage management
- **Airport Management** - Centralized airport information and operations
- **Aircraft Operations** - Track and manage aircraft inventory and maintenance
- **Staff Administration** - Manage staff accounts and user roles with secure authentication
- **Notifications** - Real-time passenger notifications for flight updates
- **Admin Dashboard** - Comprehensive admin tools for system management and announcements
- **User Authentication** - Secure login and account management for passengers and staff

## Tech Stack

### Backend
- **Framework:** .NET (C#)
- **Architecture:** ASP.NET Core with MVC pattern
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT & Identity Framework

### Frontend
- **Framework:** Angular
- **Language:** TypeScript
- **Styling:** CSS/SCSS
- **Build Tool:** Angular CLI

## Project Structure

```
MizzAir/
├── API/                          # Backend (.NET)
│   ├── Controllers/              # API endpoints
│   ├── Services/                 # Business logic
│   ├── Models/                   # Data models
│   ├── DTOs/                     # Data transfer objects
│   ├── Data/                     # Database context & migrations
│   ├── Extensions/               # Extension methods
│   ├── Middleware/               # Custom middleware
│   └── Program.cs                # Application entry point
│
└── client/                       # Frontend (Angular)
    ├── src/
    │   ├── app/                  # Angular components & services
    │   ├── assets/               # Static resources
    │   └── index.html            # Main HTML file
    └── angular.json              # Angular configuration
```

## Getting Started

### Prerequisites
- .NET SDK (v7.0 or higher)
- Node.js (v18 or higher)
- Angular CLI (`npm install -g @angular/cli`)
- SQL Server (local or cloud instance)

### Backend Setup

1. Navigate to the API directory:
   ```bash
   cd API
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update database connection string in `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "your_connection_string_here"
   }
   ```

4. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the backend server:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:5001`

### Frontend Setup

1. Navigate to the client directory:
   ```bash
   cd client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   ng serve
   ```
   The application will be available at `http://localhost:4200`

## API Documentation

The backend API provides RESTful endpoints organized by resource:
- `/api/flights` - Flight management
- `/api/bookings` - Booking operations
- `/api/passengers` - Passenger information
- `/api/aircraft` - Aircraft management
- `/api/airports` - Airport data
- `/api/accounts` - User account management
- `/api/staff` - Staff administration
- `/api/notifications` - Notifications

## Database

The application uses SQL Server with Entity Framework Core for data persistence. Seed data is provided in:
- `Data/FlightSeedData.json` - Initial flight data
- `Data/UserSeedData.json` - Initial user data

Run `Seed.cs` on application startup to populate the database with initial data.

## Key Components

### Authentication
- JWT-based token authentication for secure API access
- Role-based access control (RBAC) with different user roles
- Separate authentication flows for passengers and staff

### Data Models
- **User Accounts** - Core user entity with role support
- **Flights** - Flight schedules and routing information
- **Bookings** - Passenger booking records with seats
- **Passengers** - Individual passenger details
- **Aircraft** - Aircraft inventory and specifications
- **Airports** - Airport information and operations
- **Notifications** - System notifications for users
- **Baggage** - Baggage management and tracking

## Development Guidelines

- Follow C# naming conventions (PascalCase for classes, camelCase for properties)
- Use DTOs for API contracts
- Implement services for business logic separation
- Use Entity Framework Core migrations for database changes
- Maintain clean separation of concerns across layers

## Contributing

1. Create a feature branch from `main`
2. Make your changes with clear commit messages
3. Test thoroughly before submitting
4. Submit a pull request with a detailed description

## Support & Documentation

For more information about specific features or components, refer to the respective controller and service documentation within the codebase.

---

**Version:** 1.0.0  
**Last Updated:** May 2026
