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

        return services;
    }
}
