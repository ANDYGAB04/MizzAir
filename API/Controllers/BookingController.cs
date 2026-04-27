using System.Security.Claims;
using API.DTOs;
using API.Extensions;
using API.Interface;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BookingController(IBookingService bookingService, IMapper mapper) : BaseApiController
    {

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var userId = User.GetUserId();
            var bookings = await bookingService.GetBookingList(userId);
            var bookingsDto = mapper.Map<IEnumerable<BookingDto>>(bookings);

            return Ok(bookingsDto);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookingDto>> BookingFlight([FromBody] CreateBookingDto createBookingDto)
        {
            var userId = User.GetUserId();
            if (createBookingDto.FlightId == 0)
            {
                return BadRequest("No flight id found");
            }
            if (createBookingDto.SeatIds.Count == 0)
            {
                return BadRequest("No seat were chosen");
            }

            var booking = await bookingService.CreateBooking(userId, createBookingDto.FlightId, createBookingDto.SeatIds);
            var bookingDto = mapper.Map<BookingDto>(booking);

            return CreatedAtAction(nameof(BookingFlight), new { id = bookingDto.Id }, bookingDto);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            var userId = User.GetUserId();
            await bookingService.CancelBooking(userId, id);
            return Ok();
        }
    }
}
