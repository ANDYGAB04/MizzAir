using API.DTOs;

namespace API.Interface;

public interface IPassengerService
{
    Task<PaginatedResultDto<PassengerDto>> GetPassengersAsync(PassengerFilterDto filterDto);
    Task<PassengerDto> GetPassengerByIdAsync(int id);
    Task<DeletePassengerResultDto> DeletePassengerAsync(int id);
}
