using API.DTOs;

namespace API.Interface;

public interface IAirportService
{
    Task<List<AirportDto>> GetAllAirports();
}
