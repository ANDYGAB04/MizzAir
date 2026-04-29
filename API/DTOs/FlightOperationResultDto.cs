namespace API.DTOs;

public class FlightOperationResultDto
{
    public bool Succeeded { get; set; }
    public FlightAdminDto? Flight { get; set; }
    public List<string> Errors { get; set; } = [];
}
