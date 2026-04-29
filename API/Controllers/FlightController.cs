using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FlightController(IFlightService flightService) : BaseApiController
    {
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FlightDto>>> Search([FromQuery] SearchFlightDto searchFlightDto)
        {
            var results = await flightService.SearchFlights(searchFlightDto);
            return Ok(results);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightAdminDto>>> GetFlights()
        {
            var flights = await flightService.GetFlightsAsync();
            return Ok(flights);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightAdminDto>> GetFlight(int id)
        {
            var flight = await flightService.GetFlightByIdAsync(id);

            if (flight == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            return Ok(flight);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<ActionResult<FlightAdminDto>> CreateFlight([FromBody] CreateFlightDto dto)
        {
            var result = await flightService.CreateFlightAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Created($"/api/flight/{result.Flight!.Id}", result.Flight);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}")]
        public async Task<ActionResult<FlightAdminDto>> UpdateFlight(int id, [FromBody] UpdateFlightDto dto)
        {
            var result = await flightService.UpdateFlightAsync(id, dto);

            if (result == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Flight);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteFlightResultDto>> DeleteFlight(int id)
        {
            var result = await flightService.DeleteFlightAsync(id);

            if (result == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }
}
