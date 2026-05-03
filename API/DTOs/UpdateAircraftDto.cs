using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateAircraftDto
{
    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TotalSeats { get; set; }

    [Range(1, int.MaxValue)]
    public int SeatRows { get; set; }

    [Range(1, int.MaxValue)]
    public int SeatsPerRow { get; set; }
}
