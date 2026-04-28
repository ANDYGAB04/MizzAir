using System;

namespace API.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public required string BookingReference { get; set; }
    public DateTime BookingDate { get; set; }
    public required string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int FlightId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public List<string> SeatNumbers { get; set; } = [];
    public List<string> BaggageTypes { get; set; } = [];
}
