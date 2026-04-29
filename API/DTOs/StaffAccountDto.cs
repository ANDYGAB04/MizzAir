namespace API.DTOs;

public class StaffAccountDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public required string Address { get; set; }
    public List<string> Roles { get; set; } = [];
}
