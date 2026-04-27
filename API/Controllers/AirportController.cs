using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AirportController(IAirportService airportService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirportDto>>> GetAllAirports()
        {
            var airports = await airportService.GetAllAirports();
            return Ok(airports);
        }
    }
}
