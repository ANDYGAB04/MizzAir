using System;

namespace API.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Type { get; set; }
}

