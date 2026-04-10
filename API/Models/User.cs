using System;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User : IdentityUser<int>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Address { get; set; }
    public List<Booking> Bookings { get; set; } = [];
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}
