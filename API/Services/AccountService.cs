using API.DTOs;
using API.Interface;
using API.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public class AccountService(UserManager<User> userManager, ITokenService tokenService) : IAccountService
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
