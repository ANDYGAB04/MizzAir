namespace API.DTOs;

public class UpdateAccountResultDto
{
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; } = [];
    public UserDto? User { get; set; }
}
