using API.DTOs;

namespace API.Interface;

public interface IBaggageService
{
    Task<List<BaggageTypeDto>> GetBaggageOptions();
}
