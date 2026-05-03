using API.Data;
using API.DTOs;
using API.Interface;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AircraftService(DataContext context, IMapper mapper) : IAircraftService
{
    public async Task<List<AircraftDto>> GetAircraft()
    {
        var aircraft = await context.Aircrafts
            .OrderBy(a => a.Model)
            .ThenBy(a => a.RegistrationNumber)
            .ToListAsync();

        return mapper.Map<List<AircraftDto>>(aircraft);
    }

    public async Task<AircraftDto?> GetAircraftById(int id)
    {
        var aircraft = await context.Aircrafts.FindAsync(id);

        if (aircraft == null)
        {
            return null;
        }

        return mapper.Map<AircraftDto>(aircraft);
    }

    public async Task<AircraftOperationResultDto> CreateAircraftAsync(CreateAircraftDto dto)
    {
        var errors = await ValidateAircraftAsync(dto);
        if (errors.Count > 0)
        {
            return Failure(errors);
        }

        var aircraft = new Aircraft
        {
            Model = dto.Model.Trim(),
            RegistrationNumber = NormalizeRegistrationNumber(dto.RegistrationNumber),
            TotalSeats = dto.TotalSeats,
            SeatRows = dto.SeatRows,
            SeatsPerRow = dto.SeatsPerRow
        };

        context.Aircrafts.Add(aircraft);
        await context.SaveChangesAsync();

        return new AircraftOperationResultDto
        {
            Succeeded = true,
            Aircraft = mapper.Map<AircraftDto>(aircraft)
        };
    }

    public async Task<AircraftOperationResultDto> UpdateAircraftAsync(int id, UpdateAircraftDto dto)
    {
        var aircraft = await context.Aircrafts.FindAsync(id);

        if (aircraft == null)
        {
            return Failure(["Aircraft not found."]);
        }

        var errors = await ValidateAircraftForUpdateAsync(dto, id);
        if (errors.Count > 0)
        {
            return Failure(errors);
        }

        aircraft.Model = dto.Model.Trim();
        aircraft.RegistrationNumber = NormalizeRegistrationNumber(dto.RegistrationNumber);
        aircraft.TotalSeats = dto.TotalSeats;
        aircraft.SeatRows = dto.SeatRows;
        aircraft.SeatsPerRow = dto.SeatsPerRow;

        context.Aircrafts.Update(aircraft);
        await context.SaveChangesAsync();

        return new AircraftOperationResultDto
        {
            Succeeded = true,
            Aircraft = mapper.Map<AircraftDto>(aircraft)
        };
    }

    public async Task<DeleteAircraftResultDto?> DeleteAircraftAsync(int id)
    {
        var aircraft = await context.Aircrafts
            .Include(a => a.Flights)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aircraft == null)
        {
            return null;
        }

        if (aircraft.Flights.Count > 0)
        {
            return new DeleteAircraftResultDto
            {
                Succeeded = false,
                AircraftId = id,
                Errors = ["Cannot delete an aircraft assigned to existing flights."]
            };
        }

        context.Aircrafts.Remove(aircraft);
        await context.SaveChangesAsync();

        return new DeleteAircraftResultDto
        {
            Succeeded = true,
            AircraftId = id,
            Message = "Aircraft deleted successfully"
        };
    }

    private async Task<List<string>> ValidateAircraftAsync(CreateAircraftDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Model))
        {
            errors.Add("Model is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.RegistrationNumber))
        {
            errors.Add("Registration number is required.");
        }

        if (dto.TotalSeats != dto.SeatRows * dto.SeatsPerRow)
        {
            errors.Add("Total seats must equal seat rows multiplied by seats per row.");
        }

        var registrationNumber = NormalizeRegistrationNumber(dto.RegistrationNumber);
        if (await context.Aircrafts.AnyAsync(a => a.RegistrationNumber == registrationNumber))
        {
            errors.Add("Registration number is already used.");
        }

        return errors;
    }

    private async Task<List<string>> ValidateAircraftForUpdateAsync(UpdateAircraftDto dto, int aircraftId)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Model))
        {
            errors.Add("Model is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.RegistrationNumber))
        {
            errors.Add("Registration number is required.");
        }

        if (dto.TotalSeats != dto.SeatRows * dto.SeatsPerRow)
        {
            errors.Add("Total seats must equal seat rows multiplied by seats per row.");
        }

        var registrationNumber = NormalizeRegistrationNumber(dto.RegistrationNumber);
        if (await context.Aircrafts.AnyAsync(a => a.RegistrationNumber == registrationNumber && a.Id != aircraftId))
        {
            errors.Add("Registration number is already used.");
        }

        return errors;
    }

    private static string NormalizeRegistrationNumber(string registrationNumber)
    {
        return registrationNumber.Trim().ToUpperInvariant();
    }

    private static AircraftOperationResultDto Failure(IEnumerable<string> errors)
    {
        return new AircraftOperationResultDto
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }
}
