using System;

namespace API.DTOs;

public class AdminAnnouncementResultDto
{
    public int NotificationsCreated { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Type { get; set; }
}

