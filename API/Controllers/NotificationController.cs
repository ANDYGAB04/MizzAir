using System;
using API.Data;
using API.DTOs;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class NotificationController(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetMyNotifications([FromQuery] int take = 30)
    {
        var userId = User.GetUserId();
        take = Math.Clamp(take, 1, 200);

        var items = await context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                Type = n.Type
            })
            .ToListAsync();

        return Ok(items);
    }

    [Authorize]
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = User.GetUserId();
        var count = await context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead);
        return Ok(count);
    }

    [Authorize]
    [HttpPatch("{id}/read")]
    public async Task<ActionResult> MarkRead(int id, [FromBody] MarkNotificationReadDto dto)
    {
        var userId = User.GetUserId();

        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null)
        {
            return NotFound();
        }

        notification.IsRead = dto.IsRead;
        await context.SaveChangesAsync();

        return NoContent();
    }
}

