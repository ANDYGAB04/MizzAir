using System;
using API.Data;
using API.DTOs;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FlightService(DataContext context, IMapper mapper) : IFlightService
{
    public async Task<List<FlightDto>> SearchFlights(SearchFlightDto searchFlightDto)
    {
        var flights = context.Flights
            .Include(a => a.Aircraft)
            .Include(da => da.DepartureAirport)
            .Include(aa => aa.ArrivalAirport)
            .Where(a => a.DepartureAirportId == searchFlightDto.DepartureAirportId
                    && a.ArrivalAirportId == searchFlightDto.ArrivalAirportId
                    && a.DepartureTime >= searchFlightDto.DepartureTime
                    && a.AvailableSeats >= searchFlightDto.NumberOfPassengers
                    && a.Status != "Cancelled");

        flights = searchFlightDto.SortBy switch
        {
            "price" => flights.OrderBy(p => p.Price),
            "duration" => flights.OrderBy(d => d.Duration),
            "departure" => flights.OrderBy(d => d.DepartureTime),
            _ => flights.OrderBy(p => p.Price)
        };

        var result = await flights.ToListAsync();
        return mapper.Map<List<FlightDto>>(result);
    }
}
