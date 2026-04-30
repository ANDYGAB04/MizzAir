using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class PassengerDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Address { get; set; }
    public int TotalBookings { get; set; }
    public DateTime? LastBookingDate { get; set; }
}
