using System;

namespace API.Models;

public class Seat
{
    public int Id { get; set; }
    public required string SeatNumber { get; set; }
    public int SeatRow { get; set; }
    public List<BookingSeat> BookingSeats { get; set; } = [];

    public int AircraftId { get; set; }
    public Aircraft Aircraft { get; set; } = null!;
}
