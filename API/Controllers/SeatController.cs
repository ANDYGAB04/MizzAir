using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class SeatController(ISeatService seatService) : BaseApiController
    {
        [HttpGet("flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetSeatsByFlight(int flightId)
        {
            var seats = await seatService.GetSeatsByFlightId(flightId);
            return Ok(seats);
        }

        [HttpGet("flight/{flightId}/booked")]
        public async Task<ActionResult<IEnumerable<int>>> GetBookedSeats(int flightId)
        {
            var bookedSeats = await seatService.GetBookedSeatsByFlightId(flightId);
            return Ok(bookedSeats);
        }
    }
}
