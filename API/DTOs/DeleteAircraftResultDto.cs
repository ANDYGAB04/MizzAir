namespace API.DTOs;

public class DeleteAircraftResultDto
{
    public bool Succeeded { get; set; }
    public int AircraftId { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
}
