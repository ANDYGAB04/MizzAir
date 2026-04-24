using System;
using API.DTOs;

namespace API.Interface;

public interface IFlightService
{
    Task<List<FlightDto>> SearchFlights(SearchFlightDto searchFlightDto);
}
