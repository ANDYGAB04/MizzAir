using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AircraftController(IAircraftService aircraftService) : BaseApiController
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<AircraftDto>> GetAircraftById(int id)
        {
            var aircraft = await aircraftService.GetAircraftById(id);
            
            if (aircraft == null)
            {
                return NotFound();
            }
            
            return Ok(aircraft);
        }
    }
}
