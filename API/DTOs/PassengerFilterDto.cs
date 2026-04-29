namespace API.DTOs;

public class PassengerFilterDto
{
    // Nullable so `GET /api/passenger` works when no search term is provided.
    public string? SearchTerm { get; set; }
    public int? FlightId { get; set; }
    public string SortBy { get; set; } = "LastName";
    public bool IsDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
