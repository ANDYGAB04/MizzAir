using System;
using API.Data;
using API.Interface;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class NotificationService(DataContext context) : INotificationService
{
    public async Task<int> BroadcastAnnouncement(int adminUserId, string message, string type)
    {
        _ = adminUserId;

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required", nameof(message));
        }

        type = string.IsNullOrWhiteSpace(type) ? "Announcement" : type.Trim();
        message = message.Trim();

        var userIds = await context.Users
            .AsNoTracking()
            .Select(u => u.Id)
            .ToListAsync();

        if (userIds.Count == 0)
        {
            return 0;
        }

        var now = DateTime.UtcNow;
        var notifications = userIds.Select(userId => new Notification
        {
            Message = message,
            Type = type,
            CreatedAt = now,
            IsRead = false,
            UserId = userId
        }).ToList();

        await using var tx = await context.Database.BeginTransactionAsync();
        context.Notifications.AddRange(notifications);
        var created = await context.SaveChangesAsync();
        await tx.CommitAsync();

        return created;
    }
}

