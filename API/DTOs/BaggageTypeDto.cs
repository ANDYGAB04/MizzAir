namespace API.DTOs;

public class BaggageTypeDto
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string MaxDimensions { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal Price { get; set; }
}
