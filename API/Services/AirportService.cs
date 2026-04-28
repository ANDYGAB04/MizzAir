using API.Data;
using API.DTOs;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AirportService(DataContext context, IMapper mapper) : IAirportService
{
    public async Task<List<AirportDto>> GetAllAirports()
    {
        var airports = await context.Airports.OrderBy(a => a.City).ToListAsync();
        return mapper.Map<List<AirportDto>>(airports);
    }
}
