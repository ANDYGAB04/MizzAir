using API.DTOs;

namespace API.Interface;

public interface ISeatService
{
    Task<List<SeatDto>> GetSeatsByFlightId(int flightId);
    Task<List<int>> GetBookedSeatsByFlightId(int flightId);
}
