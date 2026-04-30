namespace API.DTOs;

public class AircraftOperationResultDto
{
    public bool Succeeded { get; set; }
    public AircraftDto? Aircraft { get; set; }
    public List<string> Errors { get; set; } = [];
}
