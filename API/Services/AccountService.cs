using API.DTOs;
using API.Data;
using API.Interface;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AccountService(UserManager<User> userManager, ITokenService tokenService, DataContext context) : IAccountService
{
    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        return await CreateUserDto(user);
    }

    public async Task<UpdateAccountResultDto> UpdateAccountAsync(int userId, UpdateAccountDto updateAccountDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new UpdateAccountResultDto
            {
                Succeeded = false,
                Errors = ["User not found"]
            };
        }

        if (!string.IsNullOrWhiteSpace(updateAccountDto.City))
        {
            user.City = updateAccountDto.City.Trim();
            user.Country = updateAccountDto.Country!.Trim();
            user.Address = updateAccountDto.Address!.Trim();
        }

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return new UpdateAccountResultDto
            {
                Succeeded = false,
                Errors = updateResult.Errors.Select(x => x.Description).ToList()
            };
        }

        if (!string.IsNullOrWhiteSpace(updateAccountDto.CurrentPassword))
        {
            var changePasswordResult = await userManager.ChangePasswordAsync(
                user,
                updateAccountDto.CurrentPassword,
                updateAccountDto.NewPassword!);

            if (!changePasswordResult.Succeeded)
            {
                return new UpdateAccountResultDto
                {
                    Succeeded = false,
                    Errors = changePasswordResult.Errors.Select(x => x.Description).ToList()
                };
            }
        }

        return new UpdateAccountResultDto
        {
            Succeeded = true,
            User = await CreateUserDto(user)
        };
    }

    public async Task<DeleteAccountResultDto?> DeleteCurrentUserAsync(int userId)
    {
        var user = await userManager.Users
            .Include(u => u.Bookings)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        var deletedAt = DateTime.UtcNow;
        var reservationsToSoftDelete = user.Bookings.Where(b => !b.IsDeleted).ToList();
        var softDeletedReservationsCount = reservationsToSoftDelete.Count;

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (softDeletedReservationsCount > 0)
            {
                foreach (var booking in reservationsToSoftDelete)
                {
                    booking.IsDeleted = true;
                    booking.DeletedAt = deletedAt;
                    booking.Status = "Cancelled";
                }
            }

            await userManager.UpdateSecurityStampAsync(user);

            user.IsDeleted = true;
            user.DeletedAt = deletedAt;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new DeleteAccountResultDto
            {
                UserId = userId,
                DeletedReservationsCount = softDeletedReservationsCount,
                DeletedSessionsCount = 0,
                DeletedAt = deletedAt,
                Message = "Account deleted successfully"
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<UserDto> CreateUserDto(User user)
    {
        return new UserDto
        {
            FirstName = user.FirstName,
            Lastname = user.LastName,
            Email = user.Email!,
            City = user.City,
            Country = user.Country,
            Address = user.Address,
            Token = await tokenService.CreateToken(user)
        };
    }
}
