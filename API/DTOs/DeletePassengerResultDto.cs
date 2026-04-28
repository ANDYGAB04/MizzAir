namespace API.DTOs;

public class DeletePassengerResultDto
{
    public int PassengerId { get; set; }
    public int DeletedReservationsCount { get; set; }
    public int DeletedSessionsCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
