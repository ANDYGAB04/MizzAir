namespace API.DTOs;

public class FlightAdminDto
{
    public int Id { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public int Duration { get; set; }
    public required string Status { get; set; }
    public int AircraftId { get; set; }
    public required string AircraftType { get; set; }
    public int DepartureAirportId { get; set; }
    public required string DepartureAirportName { get; set; }
    public int ArrivalAirportId { get; set; }
    public required string ArrivalAirportName { get; set; }
}
