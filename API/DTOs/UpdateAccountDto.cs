using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateAccountDto 
{
    [StringLength(50, MinimumLength = 2)]
    public string? City { get; set; }

    [StringLength(50, MinimumLength = 2)]
    public string? Country { get; set; }

    [StringLength(100, MinimumLength = 5)]
    public string? Address { get; set; }

    public string? CurrentPassword { get; set; }

    [StringLength(8, MinimumLength = 4)]
    public string? NewPassword { get; set; }

    
}
