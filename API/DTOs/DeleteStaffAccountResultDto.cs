namespace API.DTOs;

public class DeleteStaffAccountResultDto
{
    public bool Succeeded { get; set; }
    public int StaffId { get; set; }
    public int DeletedSessionsCount { get; set; }
    public DateTime DeletedAt { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
}
