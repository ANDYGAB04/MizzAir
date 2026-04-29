namespace API.DTOs;

public class DeleteAccountResultDto
{
    public int UserId { get; set; }
    public int DeletedReservationsCount { get; set; }
    public int DeletedSessionsCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
