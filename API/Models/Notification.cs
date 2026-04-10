using System;

namespace API.Models;

public class Notification
{
    public int Id { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Type { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
