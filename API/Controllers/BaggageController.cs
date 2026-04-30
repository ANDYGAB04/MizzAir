using API.DTOs;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BaggageController(IBaggageService baggageService) : BaseApiController
    {
        [Authorize]
        [HttpGet("options")]
        public async Task<ActionResult<IEnumerable<BaggageTypeDto>>> GetBaggageOptions()
        {
            var baggageOptions = await baggageService.GetBaggageOptions();
            return Ok(baggageOptions);
        }
    }
}
