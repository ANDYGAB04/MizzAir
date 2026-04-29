using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateStaffAccountDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    [Required]
    public string? Address { get; set; }

    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; } = string.Empty;
}
