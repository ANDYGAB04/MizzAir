using API.DTOs;

namespace API.Interface;

public interface IAircraftService
{
    Task<List<AircraftDto>> GetAircraft();
    Task<AircraftDto?> GetAircraftById(int id);
    Task<AircraftOperationResultDto> CreateAircraftAsync(CreateAircraftDto dto);
    Task<AircraftOperationResultDto> UpdateAircraftAsync(int id, UpdateAircraftDto dto);
    Task<DeleteAircraftResultDto?> DeleteAircraftAsync(int id);
}
