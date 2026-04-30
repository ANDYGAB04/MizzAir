namespace API.DTOs;

public class AircraftDto
{
    public int Id { get; set; }
    public required string Model { get; set; }
    public required string RegistrationNumber { get; set; }
    public int TotalSeats { get; set; }
    public int SeatRows { get; set; }
    public int SeatsPerRow { get; set; }
}
