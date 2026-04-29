using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AdminAnnouncementDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    // Optional override; default will be used when omitted.
    [StringLength(50)]
    public string? Type { get; set; }
}

