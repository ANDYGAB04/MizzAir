using System;
using API.DTOs;

namespace API.Interface;

public interface IFlightService
{
    Task<List<FlightDto>> SearchFlights(SearchFlightDto searchFlightDto);
    Task<List<FlightAdminDto>> GetFlightsAsync();
    Task<FlightAdminDto?> GetFlightByIdAsync(int id);
    Task<FlightOperationResultDto> CreateFlightAsync(CreateFlightDto dto);
    Task<FlightOperationResultDto?> UpdateFlightAsync(int id, UpdateFlightDto dto);
    Task<DeleteFlightResultDto?> DeleteFlightAsync(int id);
}
