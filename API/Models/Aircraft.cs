using System;

namespace API.Models;

public class Aircraft
{
    public int Id { get; set; }
    public required string Model { get; set; }
    public int TotalSeats { get; set; }
    public int SeatRows { get; set; }
    public int SeatsPerRow { get; set; }

    public List<Seat> Seats { get; set; } = [];
    public List<Flight> Flights { get; set; } = [];
}
