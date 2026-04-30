namespace API.DTOs;

public class DeleteFlightResultDto
{
    public bool Succeeded { get; set; }
    public int FlightId { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
}
