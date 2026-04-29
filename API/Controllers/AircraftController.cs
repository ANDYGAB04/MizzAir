using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AircraftController(IAircraftService aircraftService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AircraftDto>>> GetAircraft()
        {
            var aircraft = await aircraftService.GetAircraft();
            return Ok(aircraft);
        }

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

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<ActionResult<AircraftDto>> CreateAircraft([FromBody] CreateAircraftDto dto)
        {
            var result = await aircraftService.CreateAircraftAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Created($"/api/aircraft/{result.Aircraft!.Id}", result.Aircraft);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteAircraftResultDto>> DeleteAircraft(int id)
        {
            var result = await aircraftService.DeleteAircraftAsync(id);

            if (result == null)
            {
                return NotFound($"Aircraft with ID {id} not found");
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }
}
