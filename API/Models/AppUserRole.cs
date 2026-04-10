using System;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class AppUserRole : IdentityUserRole<int>
{
    public User User { get; set; } = null!;
    public AppRole Role { get; set; } = null!;
}
