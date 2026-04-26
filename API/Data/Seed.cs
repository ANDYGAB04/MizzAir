using System;
using System.Text.Json;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<User> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync())
        {
            return;
        }
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<User>>(userData, options);

        if (users == null) return;

        var roles = new List<AppRole>
        {
            new() {Name = "Member"},
            new() {Name = "Admin"},
            new() {Name = "Staff"},
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {

            user.Email = user.Email!.ToLower();
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }

        var admin = new User
        {
            FirstName = "admin",
            LastName = "admin",
            Email = "admin@gmail.com",
            Address = ""
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Admin", "Staff"]);

    }

    public static async Task SeedFlights(DataContext context)
    {
        if (await context.Flights.AnyAsync())
        {
            return;
        }

        var flightData = await File.ReadAllTextAsync("Data/FlightSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var flights = JsonSerializer.Deserialize<List<Flight>>(flightData, options);

        if (flights == null) return;

        foreach (var flight in flights)
        {
            context.Flights.Add(flight);
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedAircraft(DataContext context)
    {
        if (await context.Aircrafts.AnyAsync())
        {
            return;
        }

        var aircraft = new List<Aircraft>
        {
            new() { Model = "Boeing 737", TotalSeats = 189, SeatRows = 27, SeatsPerRow = 7 },
            new() { Model = "Airbus A320", TotalSeats = 180, SeatRows = 30, SeatsPerRow = 6 },
            new() { Model = "Airbus A380", TotalSeats = 525, SeatRows = 75, SeatsPerRow = 7 }
        };

        foreach (var item in aircraft)
        {
            context.Aircrafts.Add(item);
        }

        await context.SaveChangesAsync();
        await SeedSeats(context);
    }

    public static async Task SeedSeats(DataContext context)
    {
        if (await context.Seats.AnyAsync())
        {
            return;
        }

        var aircrafts = await context.Aircrafts.ToListAsync();
        var seatLetters = new[] { "A", "B", "C", "D", "E", "F", "G" };

        foreach (var aircraft in aircrafts)
        {
            var seats = new List<Seat>();
            var seatLetterCount = Math.Min(aircraft.SeatsPerRow, seatLetters.Length);

            for (int row = 1; row <= aircraft.SeatRows; row++)
            {
                for (int col = 0; col < seatLetterCount; col++)
                {
                    var seat = new Seat
                    {
                        SeatNumber = $"{row}{seatLetters[col]}",
                        SeatRow = row,
                        AircraftId = aircraft.Id
                    };
                    seats.Add(seat);
                }
            }

            foreach (var seat in seats)
            {
                context.Seats.Add(seat);
            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedAirports(DataContext context)
    {
        if (await context.Airports.AnyAsync())
        {
            return;
        }

        var airports = new List<Airport>
        {
            new() { Name = "John F. Kennedy International Airport", City = "New York", Country = "USA", IATACode = "JFK" },
            new() { Name = "Los Angeles International Airport", City = "Los Angeles", Country = "USA", IATACode = "LAX" },
            new() { Name = "Chicago O'Hare International Airport", City = "Chicago", Country = "USA", IATACode = "ORD" }
        };

        foreach (var airport in airports)
        {
            context.Airports.Add(airport);
        }

        await context.SaveChangesAsync();
    }
}
