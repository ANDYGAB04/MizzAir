using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateFlightDto
{
    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableSeats { get; set; }

    [Required]
    public string Status { get; set; } = "Scheduled";

    [Range(1, int.MaxValue)]
    public int AircraftId { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartureAirportId { get; set; }

    [Range(1, int.MaxValue)]
    public int ArrivalAirportId { get; set; }
}
