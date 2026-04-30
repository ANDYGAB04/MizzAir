using API.Data;
using API.DTOs;
using API.Interface;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class PassengerService(DataContext _context, UserManager<User> _userManager, IBookingService _bookingService, IMapper _mapper) : IPassengerService
{
    public async Task<PaginatedResultDto<PassengerDto>> GetPassengersAsync(PassengerFilterDto filterDto)
    {
        // Get users with Passenger role
        var passengersQuery = _userManager.Users
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Passenger"))
            .Include(u => u.Bookings)
            .AsQueryable();




        // Apply search filter (by name or email)
        if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
        {
            var searchTerm = filterDto.SearchTerm.ToLower();
            passengersQuery = passengersQuery.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm) ||
                (p.Email != null && p.Email.ToLower().Contains(searchTerm)) ||
                (p.PhoneNumber != null && p.PhoneNumber.ToLower().Contains(searchTerm)));
        }

        // Apply flight filter
        if (filterDto.FlightId.HasValue)
        {
            passengersQuery = passengersQuery.Where(p =>
                p.Bookings.Any(b => b.FlightId == filterDto.FlightId.Value));
        }

        // Apply sorting
        passengersQuery = filterDto.SortBy?.ToLower() switch
        {
            "fullname" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.LastName).ThenByDescending(p => p.FirstName)
                : passengersQuery.OrderBy(p => p.LastName).ThenBy(p => p.FirstName),
            "firstname" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.FirstName)
                : passengersQuery.OrderBy(p => p.FirstName),
            "lastname" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.LastName)
                : passengersQuery.OrderBy(p => p.LastName),
            "email" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.Email)
                : passengersQuery.OrderBy(p => p.Email),
            "phone" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.PhoneNumber)
                : passengersQuery.OrderBy(p => p.PhoneNumber),
            "totalbookings" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.Bookings.Count)
                : passengersQuery.OrderBy(p => p.Bookings.Count),
            "lastbooking" => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.Bookings.Max(b => b.BookingDate))
                : passengersQuery.OrderBy(p => p.Bookings.Max(b => b.BookingDate)),
            _ => filterDto.IsDescending
                ? passengersQuery.OrderByDescending(p => p.LastName)
                : passengersQuery.OrderBy(p => p.LastName)
        };

        // Get total count before pagination
        var totalCount = await passengersQuery.CountAsync();

        // Calculate pagination values
        var totalPages = (int)Math.Ceiling(totalCount / (double)filterDto.PageSize);
        var hasNext = filterDto.PageNumber < totalPages;
        var hasPrevious = filterDto.PageNumber > 1;

        // Apply pagination
        var passengers = await passengersQuery
            .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
            .Take(filterDto.PageSize)
            .Select(p => new PassengerDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email ?? string.Empty,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                TotalBookings = p.Bookings.Count,
                LastBookingDate = p.Bookings.Any() ? p.Bookings.Max(b => b.BookingDate) : null
            })
            .ToListAsync();

        return new PaginatedResultDto<PassengerDto>
        {
            Items = passengers,
            CurrentPage = filterDto.PageNumber,
            PageSize = filterDto.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNext = hasNext,
            HasPrevious = hasPrevious
        };
    }

    public async Task<PassengerDto> GetPassengerByIdAsync(int id)
    {
        var passenger = await _userManager.Users
            .Include(u => u.Bookings)
            .Where(u => u.Id == id && u.UserRoles.Any(ur => ur.Role.Name == "Passenger"))
            .Select(p => new PassengerDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email ?? string.Empty,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                TotalBookings = p.Bookings.Count,
                LastBookingDate = p.Bookings.Any() ? p.Bookings.Max(b => b.BookingDate) : null
            })
            .FirstOrDefaultAsync();

        if (passenger is null)
        {
            throw new NullReferenceException("Passenger not found");
        }
        return passenger;
    }

    public async Task<DeletePassengerResultDto> DeletePassengerAsync(int id)
    {
        var passenger = await _userManager.Users
            .Include(u => u.Bookings)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id && u.UserRoles.Any(ur => ur.Role.Name == "Passenger"));

        if (passenger == null)
        {
            throw new NullReferenceException("Passenger not found");
        }

        var deletedReservationsCount = passenger.Bookings.Count;
        var deletedSessionsCount =
            await _context.Set<IdentityUserLogin<int>>().CountAsync(x => x.UserId == id) +
            await _context.Set<IdentityUserToken<int>>().CountAsync(x => x.UserId == id);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (deletedReservationsCount > 0)
            {
                _context.Bookings.RemoveRange(passenger.Bookings);
            }

            var userLogins = await _context.Set<IdentityUserLogin<int>>()
                .Where(x => x.UserId == id)
                .ToListAsync();

            if (userLogins.Count > 0)
            {
                _context.Set<IdentityUserLogin<int>>().RemoveRange(userLogins);
            }

            var userTokens = await _context.Set<IdentityUserToken<int>>()
                .Where(x => x.UserId == id)
                .ToListAsync();

            if (userTokens.Count > 0)
            {
                _context.Set<IdentityUserToken<int>>().RemoveRange(userTokens);
            }

            await _context.SaveChangesAsync();

            var deleteResult = await _userManager.DeleteAsync(passenger);

            if (!deleteResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
            }

            await transaction.CommitAsync();

            return new DeletePassengerResultDto
            {
                PassengerId = id,
                DeletedReservationsCount = deletedReservationsCount,
                DeletedSessionsCount = deletedSessionsCount,
                Message = "Passenger account deleted successfully"
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetPassengerHistoryAsync(int passengerId)
    {
        var bookings = await _bookingService.GetBookingList(passengerId);

        // Keep the newest bookings first for the staff/admin passenger profile view.
        var orderedBookings = bookings.OrderByDescending(b => b.BookingDate).ToList();

        return _mapper.Map<IEnumerable<BookingDto>>(orderedBookings);
    }

    public async Task<PassengerProfileDto?> GetPassengerProfileAsync(int passengerId)
    {
        var passenger = await GetPassengerByIdAsync(passengerId);
        if (passenger == null)
        {
            return null;
        }

        var history = await GetPassengerHistoryAsync(passengerId);

        return new PassengerProfileDto
        {
            Passenger = passenger,
            Reservations = history.ToList()
        };
    }
}
