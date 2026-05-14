using Xunit;
using API.Models;
using API.DTOs;

namespace API.Tests.Models;

public class ModelInstantiationTests
{
    [Fact]
    public void User_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St"
        };

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void Airport_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var airport = new Airport
        {
            Id = 1,
            Name = "John F. Kennedy",
            City = "New York",
            Country = "USA",
            IATACode = "JFK"
        };

        // Assert
        Assert.NotNull(airport);
        Assert.Equal("John F. Kennedy", airport.Name);
        Assert.Equal("New York", airport.City);
    }

    [Fact]
    public void BaggageType_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var baggage = new BaggageType
        {
            Id = 1,
            Type = "Carry-on",
            MaxDimensions = "55x40x20cm",
            MaxWeight = 7
        };

        // Assert
        Assert.NotNull(baggage);
        Assert.Equal("Carry-on", baggage.Type);
        Assert.Equal("55x40x20cm", baggage.MaxDimensions);
        Assert.Equal(7, baggage.MaxWeight);
    }

    [Fact]
    public void Booking_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var booking = new Booking
        {
            Id = 1,
            BookingReference = "BK123456",
            Status = "Confirmed",
            TotalPrice = 299.99m
        };

        // Assert
        Assert.NotNull(booking);
        Assert.Equal("BK123456", booking.BookingReference);
        Assert.Equal("Confirmed", booking.Status);
        Assert.Equal(299.99m, booking.TotalPrice);
    }

    [Fact]
    public void Flight_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var flight = new Flight
        {
            Id = 1,
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(2),
            Status = "On Time",
            Price = 299.99m,
            AvailableSeats = 50
        };

        // Assert
        Assert.NotNull(flight);
        Assert.Equal("On Time", flight.Status);
        Assert.Equal(50, flight.AvailableSeats);
    }

    [Fact]
    public void Aircraft_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var aircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 747",
            RegistrationNumber = "N12345",
            TotalSeats = 400,
            SeatRows = 20,
            SeatsPerRow = 20
        };

        // Assert
        Assert.NotNull(aircraft);
        Assert.Equal("Boeing 747", aircraft.Model);
        Assert.Equal("N12345", aircraft.RegistrationNumber);
        Assert.Equal(400, aircraft.TotalSeats);
    }

    [Fact]
    public void Notification_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var notification = new Notification
        {
            Id = 1,
            Message = "Test notification",
            IsRead = false,
            Type = "Info"
        };

        // Assert
        Assert.NotNull(notification);
        Assert.Equal("Test notification", notification.Message);
        Assert.False(notification.IsRead);
    }

    [Fact]
    public void Seat_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var seat = new Seat
        {
            Id = 1,
            SeatNumber = "12A",
            SeatRow = 12
        };

        // Assert
        Assert.NotNull(seat);
        Assert.Equal("12A", seat.SeatNumber);
        Assert.Equal(12, seat.SeatRow);
    }

    [Fact]
    public void AppRole_Model_CanBeInstantiated()
    {
        // Arrange & Act
        var role = new AppRole
        {
            Id = 1,
            Name = "Admin"
        };

        // Assert
        Assert.NotNull(role);
        Assert.Equal("Admin", role.Name);
    }
}
