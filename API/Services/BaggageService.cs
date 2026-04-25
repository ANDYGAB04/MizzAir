using API.Data;
using API.DTOs;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class BaggageService(DataContext context, IMapper mapper) : IBaggageService
{
    public async Task<List<BaggageTypeDto>> GetBaggageOptions()
    {
        var baggageTypes = await context.BaggageTypes.ToListAsync();
        return mapper.Map<List<BaggageTypeDto>>(baggageTypes);
    }
}
