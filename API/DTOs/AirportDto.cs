namespace API.DTOs;

public class AirportDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string IATACode { get; set; }
}
