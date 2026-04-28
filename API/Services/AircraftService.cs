using API.Data;
using API.DTOs;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AircraftService(DataContext context, IMapper mapper) : IAircraftService
{
    public async Task<AircraftDto?> GetAircraftById(int id)
    {
        var aircraft = await context.Aircrafts.FindAsync(id);

        if (aircraft == null)
        {
            return null;
        }

        return mapper.Map<AircraftDto>(aircraft);
    }
}
