using System;

namespace API.Models;

public class Flight
{
    public int Id { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int Duration { get; set; }
    public required string Status { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public List<Booking> Bookings { get; set; } = [];

    public int AircraftId { get; set; }
    public Aircraft Aircraft { get; set; } = null!;
    public int DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = null!;
    public int ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = null!;

}
