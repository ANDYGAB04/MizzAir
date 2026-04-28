using API.DTOs;

namespace API.Interface;

public interface IAircraftService
{
    Task<AircraftDto?> GetAircraftById(int id);
}
