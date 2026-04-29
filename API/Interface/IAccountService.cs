using API.DTOs;

namespace API.Interface;

public interface IAccountService
{
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<UpdateAccountResultDto> UpdateAccountAsync(int userId, UpdateAccountDto updateAccountDto);
}
