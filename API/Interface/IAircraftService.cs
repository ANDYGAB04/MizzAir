using API.DTOs;

namespace API.Interface;

public interface IAircraftService
{
    Task<List<AircraftDto>> GetAircraft();
    Task<AircraftDto?> GetAircraftById(int id);
}
