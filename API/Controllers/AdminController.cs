using System;
using API.DTOs;
using API.Extensions;
using API.Interface;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<User> userManager, INotificationService notificationService) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
                .OrderBy(x => x.Email)
                .Select(x => new
                {
                    x.Id,
                    Email = x.Email,
                    Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
                }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("broadcast-announcement")]
    public async Task<ActionResult<AdminAnnouncementResultDto>> BroadcastAnnouncement([FromBody] AdminAnnouncementDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            return BadRequest("Message is required");
        }

        var adminUserId = User.GetUserId();
        var type = string.IsNullOrWhiteSpace(dto.Type) ? "Announcement" : dto.Type;
        var created = await notificationService.BroadcastAnnouncement(adminUserId, dto.Message, type);

        return Ok(new AdminAnnouncementResultDto
        {
            NotificationsCreated = created,
            CreatedAt = DateTime.UtcNow,
            Type = string.IsNullOrWhiteSpace(type) ? "Announcement" : type.Trim()
        });
    }
}
