using System;
using API.Data;
using API.DTOs;
using API.Interface;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FlightService(DataContext context, IMapper mapper, INotificationService notificationService) : IFlightService
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Scheduled",
        "Active",
        "Delayed",
        "Cancelled",
        "Completed"
    };

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

    public async Task<List<FlightAdminDto>> GetFlightsAsync()
    {
        var flights = await context.Flights
            .Include(f => f.Aircraft)
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .OrderBy(f => f.DepartureTime)
            .ToListAsync();

        return mapper.Map<List<FlightAdminDto>>(flights);
    }

    public async Task<FlightAdminDto?> GetFlightByIdAsync(int id)
    {
        var flight = await GetFlightWithIncludes(id);
        return flight == null ? null : mapper.Map<FlightAdminDto>(flight);
    }

    public async Task<FlightOperationResultDto> CreateFlightAsync(CreateFlightDto dto)
    {
        var validationErrors = await ValidateFlightAsync(dto, null);
        if (validationErrors.Count > 0)
        {
            return Failure(validationErrors);
        }

        var flight = new Flight
        {
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            Duration = CalculateDuration(dto.DepartureTime, dto.ArrivalTime),
            Status = NormalizeStatus(dto.Status),
            Price = dto.Price,
            AvailableSeats = dto.AvailableSeats,
            AircraftId = dto.AircraftId,
            DepartureAirportId = dto.DepartureAirportId,
            ArrivalAirportId = dto.ArrivalAirportId
        };

        context.Flights.Add(flight);
        await context.SaveChangesAsync();

        var createdFlight = await GetFlightWithIncludes(flight.Id);

        return new FlightOperationResultDto
        {
            Succeeded = true,
            Flight = mapper.Map<FlightAdminDto>(createdFlight)
        };
    }

    public async Task<FlightOperationResultDto?> UpdateFlightAsync(int id, UpdateFlightDto dto)
    {
        var flight = await context.Flights
            .Include(f => f.Bookings)
                .ThenInclude(b => b.BookingSeats)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flight == null)
        {
            return null;
        }

        var validationErrors = await ValidateFlightAsync(dto, flight);
        if (validationErrors.Count > 0)
        {
            return Failure(validationErrors);
        }

        var oldStatus = flight.Status;
        var statusChanged = !oldStatus.Equals(NormalizeStatus(dto.Status), StringComparison.OrdinalIgnoreCase);

        flight.DepartureTime = dto.DepartureTime;
        flight.ArrivalTime = dto.ArrivalTime;
        flight.Duration = CalculateDuration(dto.DepartureTime, dto.ArrivalTime);
        flight.Status = NormalizeStatus(dto.Status);
        flight.Price = dto.Price;
        flight.AvailableSeats = dto.AvailableSeats;
        flight.AircraftId = dto.AircraftId;
        flight.DepartureAirportId = dto.DepartureAirportId;
        flight.ArrivalAirportId = dto.ArrivalAirportId;

        await context.SaveChangesAsync();

        // Notify passengers if status changed
        if (statusChanged)
        {
            var flightNumber = $"FLT-{id}";
            var newStatus = flight.Status;
            await notificationService.NotifyFlightStatusChange(id, flightNumber, newStatus);
        }

        var updatedFlight = await GetFlightWithIncludes(id);

        return new FlightOperationResultDto
        {
            Succeeded = true,
            Flight = mapper.Map<FlightAdminDto>(updatedFlight)
        };
    }

    public async Task<DeleteFlightResultDto?> DeleteFlightAsync(int id)
    {
        var flight = await context.Flights
            .Include(f => f.Bookings)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flight == null)
        {
            return null;
        }

        if (flight.Bookings.Any(b => !b.IsDeleted))
        {
            return new DeleteFlightResultDto
            {
                Succeeded = false,
                FlightId = id,
                Errors = ["Cannot delete a flight with active bookings. Set the flight status to Cancelled instead."]
            };
        }

        context.Flights.Remove(flight);
        await context.SaveChangesAsync();

        return new DeleteFlightResultDto
        {
            Succeeded = true,
            FlightId = id,
            Message = "Flight deleted successfully"
        };
    }

    private async Task<List<string>> ValidateFlightAsync(CreateFlightDto dto, Flight? existingFlight)
    {
        var errors = new List<string>();

        if (dto.DepartureAirportId == dto.ArrivalAirportId)
        {
            errors.Add("Departure and arrival airports must be different.");
        }

        if (dto.ArrivalTime <= dto.DepartureTime)
        {
            errors.Add("Arrival time must be after departure time.");
        }

        if (dto.Price <= 0)
        {
            errors.Add("Price must be greater than zero.");
        }

        if (!AllowedStatuses.Contains(dto.Status.Trim()))
        {
            errors.Add("Status must be one of: Scheduled, Active, Delayed, Cancelled, Completed.");
        }

        var airportsCount = await context.Airports
            .CountAsync(a => a.Id == dto.DepartureAirportId || a.Id == dto.ArrivalAirportId);

        if (airportsCount < 2)
        {
            errors.Add("Departure and arrival airports must both exist.");
        }

        var aircraft = await context.Aircrafts.FindAsync(dto.AircraftId);
        if (aircraft == null)
        {
            errors.Add("Aircraft must exist.");
            return errors;
        }

        var bookedSeats = existingFlight == null ? 0 : existingFlight.Bookings
            .Where(b => !b.IsDeleted)
            .SelectMany(b => b.BookingSeats)
            .Count();

        var maximumAvailableSeats = aircraft.TotalSeats - bookedSeats;
        if (dto.AvailableSeats > maximumAvailableSeats)
        {
            errors.Add($"Available seats cannot exceed {maximumAvailableSeats} for the selected aircraft.");
        }

        return errors;
    }

    private Task<List<string>> ValidateFlightAsync(UpdateFlightDto dto, Flight? existingFlight)
    {
        return ValidateFlightAsync(new CreateFlightDto
        {
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            Price = dto.Price,
            AvailableSeats = dto.AvailableSeats,
            Status = dto.Status,
            AircraftId = dto.AircraftId,
            DepartureAirportId = dto.DepartureAirportId,
            ArrivalAirportId = dto.ArrivalAirportId
        }, existingFlight);
    }

    private async Task<Flight?> GetFlightWithIncludes(int id)
    {
        return await context.Flights
            .Include(f => f.Aircraft)
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    private static int CalculateDuration(DateTime departureTime, DateTime arrivalTime)
    {
        return (int)Math.Round((arrivalTime - departureTime).TotalMinutes);
    }

    private static string NormalizeStatus(string status)
    {
        var trimmed = status.Trim();
        return AllowedStatuses.First(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase));
    }

    private static FlightOperationResultDto Failure(IEnumerable<string> errors)
    {
        return new FlightOperationResultDto
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }
}
