using System;

namespace API.DTOs;

public class SearchFlightDto
{
    public required int DepartureAirportId { get; set; }
    public required int ArrivalAirportId { get; set; }
    public required DateTime DepartureTime { get; set; }
    public required int NumberOfPassengers { get; set; }
    public string? SortBy { get; set; }
}
