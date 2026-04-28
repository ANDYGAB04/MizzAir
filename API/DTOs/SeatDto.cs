namespace API.DTOs;

public class SeatDto
{
    public int Id { get; set; }
    public required string SeatNumber { get; set; }
    public int SeatRow { get; set; }
    public int AircraftId { get; set; }
}
