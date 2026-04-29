using System;
using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<ITokenService, TokenService>();
        services.AddCors();
        services.AddAutoMapper(config => { }, typeof(Program).Assembly);
        services.AddScoped<IFlightService, FlightService>();
        services.AddScoped<IBaggageService, BaggageService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IAirportService, AirportService>();
        services.AddScoped<ISeatService, SeatService>();
        services.AddScoped<IAircraftService, AircraftService>();
        services.AddScoped<IPassengerService, PassengerService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
