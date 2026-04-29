using API.DTOs;

namespace API.Interface;

public interface IStaffService
{
    Task<CreateStaffAccountResultDto> CreateStaffAccountAsync(CreateStaffAccountDto dto);
    Task<DeleteStaffAccountResultDto?> DeleteStaffAccountAsync(int id);
}
