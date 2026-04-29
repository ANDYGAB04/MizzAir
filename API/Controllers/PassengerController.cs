using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Policy = "RequireStaffOrAdminRole")]
public class PassengerController : BaseApiController
{
    private readonly IPassengerService _passengerService;

    public PassengerController(IPassengerService passengerService)
    {
        _passengerService = passengerService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResultDto<PassengerDto>>> GetPassengers([FromQuery] PassengerFilterDto filterDto)
    {
        var result = await _passengerService.GetPassengersAsync(filterDto);
        
        if (result == null || result.Items.Count == 0)
        {
            return NotFound("No passengers found with the specified criteria");
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PassengerDto>> GetPassenger(int id)
    {
        var passenger = await _passengerService.GetPassengerByIdAsync(id);
        
        if (passenger == null)
        {
            return NotFound($"Passenger with ID {id} not found");
        }

        return Ok(passenger);
    }

    [HttpGet("{id}/history")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetPassengerHistory(int id)
    {
        var history = await _passengerService.GetPassengerHistoryAsync(id);
        return Ok(history);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletePassengerResultDto>> DeletePassenger(int id)
    {
        var result = await _passengerService.DeletePassengerAsync(id);

        if (result == null)
        {
            return NotFound($"Passenger with ID {id} not found");
        }

        return Ok(result);
    }
}
