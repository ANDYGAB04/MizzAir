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

public class AdminController(
    UserManager<User> userManager,
    INotificationService notificationService,
    IStaffService staffService) : BaseApiController
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

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("staff")]
    public async Task<ActionResult<IReadOnlyList<StaffAccountDto>>> GetStaffAccounts()
    {
        var staff = await staffService.GetStaffAccountsAsync();
        return Ok(staff);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("staff")]
    public async Task<ActionResult<StaffAccountDto>> CreateStaffAccount([FromBody] CreateStaffAccountDto dto)
    {
        var result = await staffService.CreateStaffAccountAsync(dto);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Created($"/api/admin/staff/{result.Staff!.Id}", result.Staff);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("staff/{id}")]
    public async Task<ActionResult<DeleteStaffAccountResultDto>> DeleteStaffAccount(int id)
    {
        var result = await staffService.DeleteStaffAccountAsync(id);

        if (result == null)
        {
            return NotFound($"Staff account with ID {id} not found");
        }

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result);
    }
}
