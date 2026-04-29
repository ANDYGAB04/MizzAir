using API.DTOs;

namespace API.Interface;

public interface IPassengerService
{
    Task<PaginatedResultDto<PassengerDto>> GetPassengersAsync(PassengerFilterDto filterDto);
    Task<PassengerDto> GetPassengerByIdAsync(int id);
    Task<IEnumerable<BookingDto>> GetPassengerHistoryAsync(int passengerId);
    Task<PassengerProfileDto?> GetPassengerProfileAsync(int passengerId);
    Task<DeletePassengerResultDto> DeletePassengerAsync(int id);
}
