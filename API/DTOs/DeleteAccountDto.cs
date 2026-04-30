using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class DeleteAccountDto
{
    [Required]
    public bool Confirm { get; set; }
}

