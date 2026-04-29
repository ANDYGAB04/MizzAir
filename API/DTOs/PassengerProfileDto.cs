using System.Collections.Generic;

namespace API.DTOs;

public class PassengerProfileDto
{
    public required PassengerDto Passenger { get; set; }
    public List<BookingDto> Reservations { get; set; } = [];
}

