using System;

namespace API.DTOs;

public class UserDto
{
    public required string FirstName { get; set; }
    public required string Lastname { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string Address { get; set; }
    public required string Token { get; set; }
}
