using API.DTOs;

namespace API.Interface;

public interface IStaffService
{
    Task<IReadOnlyList<StaffAccountDto>> GetStaffAccountsAsync();
    Task<CreateStaffAccountResultDto> CreateStaffAccountAsync(CreateStaffAccountDto dto);
    Task<DeleteStaffAccountResultDto?> DeleteStaffAccountAsync(int id);
}
