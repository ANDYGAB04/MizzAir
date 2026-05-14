using Xunit;
using API.DTOs;

namespace API.Tests.DTOs;

public class DtoPropertyTests
{
    [Fact]
    public void AirportDto_Properties_CanBeSet()
    {
        // Arrange & Act
        var airportDto = new AirportDto
        {
            Id = 1,
            Name = "Los Angeles International",
            City = "Los Angeles",
            Country = "USA",
            IATACode = "LAX"
        };

        // Assert
        Assert.NotNull(airportDto);
        Assert.Equal(1, airportDto.Id);
        Assert.Equal("Los Angeles International", airportDto.Name);
        Assert.Equal("Los Angeles", airportDto.City);
    }

    [Fact]
    public void BaggageTypeDto_Properties_CanBeSet()
    {
        // Arrange & Act
        var baggageDto = new BaggageTypeDto
        {
            Id = 1,
            Type = "Checked Baggage",
            MaxDimensions = "80x60x40cm",
            MaxWeight = 23
        };

        // Assert
        Assert.NotNull(baggageDto);
        Assert.Equal("Checked Baggage", baggageDto.Type);
        Assert.Equal("80x60x40cm", baggageDto.MaxDimensions);
        Assert.Equal(23, baggageDto.MaxWeight);
    }

    [Fact]
    public void FlightDto_Properties_CanBeSet()
    {
        // Arrange & Act
        var flightDto = new FlightDto
        {
            Id = 1,
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(3),
            AvailableSeats = 50,
            Price = 150.00m,
            Status = "On Time",
            DepartureAirportName = "JFK",
            ArrivalAirportName = "LAX",
            AircraftType = "Boeing 747"
        };

        // Assert
        Assert.NotNull(flightDto);
        Assert.Equal("Boeing 747", flightDto.AircraftType);
        Assert.Equal(50, flightDto.AvailableSeats);
        Assert.Equal(150.00m, flightDto.Price);
    }

    [Fact]
    public void CreateFlightDto_Properties_CanBeSet()
    {
        // Arrange & Act
        var createFlightDto = new CreateFlightDto
        {
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(3),
            Price = 250.00m,
            AvailableSeats = 100,
            Status = "Scheduled",
            AircraftId = 2,
            DepartureAirportId = 1,
            ArrivalAirportId = 2
        };

        // Assert
        Assert.NotNull(createFlightDto);
        Assert.Equal(2, createFlightDto.AircraftId);
        Assert.Equal(100, createFlightDto.AvailableSeats);
        Assert.Equal("Scheduled", createFlightDto.Status);
    }

    [Fact]
    public void PassengerDto_Properties_CanBeSet()
    {
        // Arrange & Act
        var passengerDto = new PassengerDto
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Address = "456 Oak St",
            TotalBookings = 5,
            PhoneNumber = "555-1234"
        };

        // Assert
        Assert.NotNull(passengerDto);
        Assert.Equal("Jane", passengerDto.FirstName);
        Assert.Equal("jane@example.com", passengerDto.Email);
        Assert.Equal(5, passengerDto.TotalBookings);
    }

    [Fact]
    public void NotificationDto_Properties_CanBeSet()
    {
        // Arrange & Act
        var notificationDto = new NotificationDto
        {
            Id = 1,
            Message = "Flight delayed",
            IsRead = false,
            Type = "Alert"
        };

        // Assert
        Assert.NotNull(notificationDto);
        Assert.Equal("Flight delayed", notificationDto.Message);
        Assert.False(notificationDto.IsRead);
        Assert.Equal("Alert", notificationDto.Type);
    }
}
