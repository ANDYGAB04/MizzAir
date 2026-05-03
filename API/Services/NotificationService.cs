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

    public async Task<int> NotifyFlightStatusChange(int flightId, string flightNumber, string newStatus)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
        {
            throw new ArgumentException("Flight number is required", nameof(flightNumber));
        }

        if (string.IsNullOrWhiteSpace(newStatus))
        {
            throw new ArgumentException("Status is required", nameof(newStatus));
        }

        // Get all unique user IDs from non-deleted bookings for this flight
        var userIds = await context.Bookings
            .AsNoTracking()
            .Where(b => b.FlightId == flightId && !b.IsDeleted)
            .Select(b => b.UserId)
            .Distinct()
            .ToListAsync();

        if (userIds.Count == 0)
        {
            return 0;
        }

        var message = $"Flight {flightNumber} status has been updated to: {newStatus}";
        var type = "Flight Status Update";
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

