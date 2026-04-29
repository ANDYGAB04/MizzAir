using API.Data;
using API.DTOs;
using API.Interface;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class StaffService(
    UserManager<User> userManager,
    RoleManager<AppRole> roleManager,
    DataContext context) : IStaffService
{
    private const string StaffRole = "Staff";
    private const string AdminRole = "Admin";

    public async Task<CreateStaffAccountResultDto> CreateStaffAccountAsync(CreateStaffAccountDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await userManager.Users.AnyAsync(x => x.Email == email))
        {
            return Failure("Email is taken");
        }

        if (!await roleManager.RoleExistsAsync(StaffRole))
        {
            return Failure("Staff role does not exist");
        }

        var user = new User
        {
            FirstName = dto.FirstName!.Trim(),
            LastName = dto.LastName!.Trim(),
            Email = email,
            UserName = email,
            City = dto.City.Trim(),
            Country = dto.Country.Trim(),
            Address = dto.Address!.Trim()
        };

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var createResult = await userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failure(createResult.Errors.Select(x => x.Description));
            }

            var roleResult = await userManager.AddToRoleAsync(user, StaffRole);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failure(roleResult.Errors.Select(x => x.Description));
            }

            await transaction.CommitAsync();

            return new CreateStaffAccountResultDto
            {
                Succeeded = true,
                Staff = await CreateStaffDto(user)
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DeleteStaffAccountResultDto?> DeleteStaffAccountAsync(int id)
    {
        var staff = await userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                !u.IsDeleted &&
                u.UserRoles.Any(ur => ur.Role.Name == StaffRole) &&
                !u.UserRoles.Any(ur => ur.Role.Name == AdminRole));

        if (staff == null)
        {
            return null;
        }

        var deletedSessionsCount =
            await context.Set<IdentityUserLogin<int>>().CountAsync(x => x.UserId == id) +
            await context.Set<IdentityUserToken<int>>().CountAsync(x => x.UserId == id);

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var deletedAt = DateTime.UtcNow;

            await userManager.UpdateSecurityStampAsync(staff);

            staff.IsDeleted = true;
            staff.DeletedAt = deletedAt;
            staff.LockoutEnabled = true;
            staff.LockoutEnd = DateTimeOffset.MaxValue;

            var updateResult = await userManager.UpdateAsync(staff);
            if (!updateResult.Succeeded)
            {
                return new DeleteStaffAccountResultDto
                {
                    Succeeded = false,
                    StaffId = id,
                    DeletedSessionsCount = deletedSessionsCount,
                    DeletedAt = deletedAt,
                    Errors = updateResult.Errors.Select(x => x.Description).ToList()
                };
            }

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new DeleteStaffAccountResultDto
            {
                Succeeded = true,
                StaffId = id,
                DeletedSessionsCount = deletedSessionsCount,
                DeletedAt = deletedAt,
                Message = "Staff account deleted successfully"
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<StaffAccountDto> CreateStaffDto(User user)
    {
        var roles = await userManager.GetRolesAsync(user);

        return new StaffAccountDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            City = user.City,
            Country = user.Country,
            Address = user.Address,
            Roles = roles.ToList()
        };
    }

    private static CreateStaffAccountResultDto Failure(string error)
    {
        return Failure([error]);
    }

    private static CreateStaffAccountResultDto Failure(IEnumerable<string> errors)
    {
        return new CreateStaffAccountResultDto
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }
}
