using System;

namespace API.DTOs;

public class CreateBookingDto
{
    public int FlightId { get; set; }
    public List<int> SeatIds { get; set; } = [];
}
