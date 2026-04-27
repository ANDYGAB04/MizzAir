using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interface;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class BookingService(DataContext context) : IBookingService
{
    public async Task<IEnumerable<Booking>> GetBookingList(int userId)
    {
        var bookings = await context.Bookings.Where(x => x.UserId == userId).ToListAsync();

        return bookings;
    }

    public async Task CancelBooking(int userId, int bookingId)
    {
        var booking = await context.Bookings
            .Include(b => b.BookingSeats)
            .Include(b => b.Flight)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking is null)
        {
            throw new Exception("Booking not found");
        }
        if (booking.UserId != userId)
        {
            throw new UnauthorizedAccessException("User not authorized to cancel this booking");
        }

        var seatsCount = booking.BookingSeats.Count;
        var flight = booking.Flight;
        flight.AvailableSeats += seatsCount;

        context.Bookings.Remove(booking);
        await context.SaveChangesAsync();
    }

    public async Task<Booking> CreateBooking(int userId, int flightId, List<int> seatIds)
    {
        // Validate seat list is not empty
        if (seatIds.Count == 0)
        {
            throw new Exception("Must select at least one seat");
        }

        var flight = await context.Flights.FindAsync(flightId);

        if (flight is null)
        {
            throw new Exception("flight not found");
        }

        if (flight.Status != "Active" && flight.Status != "Scheduled")
        {
            throw new Exception("flight is not active");
        }

        if (flight.AvailableSeats < seatIds.Count)
        {
            throw new Exception("flight has not enough available seats");
        }

        await ValidateSeatsAvailable(flightId, seatIds);

        var user = await context.Users.FindAsync(userId);

        if (user is null)
        {
            throw new Exception("User not found");
        }

        var reference = GenerateBookingReference();


        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var booking = new Booking
                {
                    BookingReference = reference,
                    BookingDate = DateTime.UtcNow,
                    Status = "Confirmed",
                    TotalPrice = flight.Price * seatIds.Count,
                    UserId = user.Id,
                    FlightId = flightId
                };


                context.Bookings.Add(booking);
                await context.SaveChangesAsync();

                foreach (var seatId in seatIds)
                {
                    var bookingSeat = new BookingSeat
                    {
                        BookingId = booking.Id,
                        SeatId = seatId
                    };
                    context.BookingSeats.Add(bookingSeat);
                }
                await context.SaveChangesAsync();


                flight.AvailableSeats -= seatIds.Count;
                await context.SaveChangesAsync();


                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public string GenerateBookingReference()
    {
        string today = DateTime.Today.ToString("yyyyMMdd");

        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var sb = new StringBuilder(6);

        for (int i = 0; i < 6; i++)
            sb.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);

        var bookingreference = "BK" + "-" + today + "-" + sb;

        return bookingreference;
    }

    public async Task UpdateFlightInventory(int flightId, int seatsBooked)
    {
        var flight = await context.Flights.FindAsync(flightId);

        if (flight is null)
        {
            throw new Exception("flight not found");
        }

        flight.AvailableSeats -= seatsBooked;

        await context.SaveChangesAsync();
    }

    public async Task<bool> ValidateSeatsAvailable(int flightId, List<int> seatIds)
    {

        if (seatIds.Distinct().Count() != seatIds.Count)
        {
            throw new Exception("Duplicate seat IDs in request");
        }

        var flight = await context.Flights.FindAsync(flightId);
        if (flight is null)
        {
            throw new Exception("Flight not found");
        }

        var requestedSeats = await context.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();


        if (requestedSeats.Count != seatIds.Count)
        {
            throw new Exception("One or more seats do not exist");
        }


        foreach (var seat in requestedSeats)
        {
            if (seat.AircraftId != flight.AircraftId)
            {
                throw new Exception($"Seat {seat.SeatNumber} does not belong to this flight's aircraft");
            }
        }

        var bookedSeatIds = await context.BookingSeats
            .Include(bs => bs.Booking)
            .Where(bs => bs.Booking.FlightId == flightId)
            .Select(bs => bs.SeatId)
            .ToListAsync();

        foreach (var seatId in seatIds)
        {
            if (bookedSeatIds.Contains(seatId))
            {
                throw new Exception($"Seat is already booked");
            }
        }

        return true;
    }
}
