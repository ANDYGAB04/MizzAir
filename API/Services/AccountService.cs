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

        var deletedReservationsCount = user.Bookings.Count;
        var deletedSessionsCount =
            await context.Set<IdentityUserLogin<int>>().CountAsync(x => x.UserId == userId) +
            await context.Set<IdentityUserToken<int>>().CountAsync(x => x.UserId == userId);

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (deletedReservationsCount > 0)
            {
                context.Bookings.RemoveRange(user.Bookings);
            }

            var userLogins = await context.Set<IdentityUserLogin<int>>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (userLogins.Count > 0)
            {
                context.Set<IdentityUserLogin<int>>().RemoveRange(userLogins);
            }

            var userTokens = await context.Set<IdentityUserToken<int>>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (userTokens.Count > 0)
            {
                context.Set<IdentityUserToken<int>>().RemoveRange(userTokens);
            }

            await context.SaveChangesAsync();

            var deleteResult = await userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
            }

            await transaction.CommitAsync();

            return new DeleteAccountResultDto
            {
                UserId = userId,
                DeletedReservationsCount = deletedReservationsCount,
                DeletedSessionsCount = deletedSessionsCount,
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
