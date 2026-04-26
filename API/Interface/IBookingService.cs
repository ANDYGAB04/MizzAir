using System;
using API.Models;

namespace API.Interface;

public interface IBookingService
{
    Task<Booking> CreateBooking(int userId, int flightId, List<int> seatIds);
    string GenerateBookingReference();
    Task<bool> ValidateSeatsAvailable(int flightId, List<int> seatIds);
    Task UpdateFlightInventory(int flightId, int seatsBooked);
}
