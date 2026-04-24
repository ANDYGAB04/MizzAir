using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FlightController(IFlightService flightService) : BaseApiController
    {
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FlightDto>>> Search(SearchFlightDto searchFlightDto)
        {
            var results = await flightService.SearchFlights(searchFlightDto);
            return Ok(results);
        }
    }
}
