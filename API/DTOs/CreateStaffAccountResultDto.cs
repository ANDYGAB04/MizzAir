namespace API.DTOs;

public class CreateStaffAccountResultDto
{
    public bool Succeeded { get; set; }
    public StaffAccountDto? Staff { get; set; }
    public List<string> Errors { get; set; } = [];
}
