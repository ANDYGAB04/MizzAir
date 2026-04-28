using API.Data;
using API.DTOs;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class SeatService(DataContext context, IMapper mapper) : ISeatService
{
    public async Task<List<SeatDto>> GetSeatsByFlightId(int flightId)
    {
        // Get flight to find aircraft
        var flight = await context.Flights
            .Include(f => f.Aircraft)
            .FirstOrDefaultAsync(f => f.Id == flightId);

        if (flight == null)
        {
            return new List<SeatDto>();
        }

        // Get all seats for that aircraft
        var seats = await context.Seats
            .Where(s => s.AircraftId == flight.AircraftId)
            .OrderBy(s => s.SeatRow)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();

        return mapper.Map<List<SeatDto>>(seats);
    }

    public async Task<List<int>> GetBookedSeatsByFlightId(int flightId)
    {
        // Get all booked seat ids for a flight
        var bookedSeats = await context.BookingSeats
            .Where(bs => bs.Booking.FlightId == flightId)
            .Select(bs => bs.SeatId)
            .ToListAsync();

        return bookedSeats;
    }
}
